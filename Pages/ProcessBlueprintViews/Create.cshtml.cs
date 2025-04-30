using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Model;

namespace Replay.Pages.ProcessBlueprintViews
{
    /*
     * Explanation: We use a separate TaskBlueprintTemp class and a List which stores them, to keep track of all Tasks, we
     * want to create, when we finally create the ProcessBlueprint.
     * Storing a List of TaskBlueprints causes errors with ef context tracking, because we reload pages and context while creation
     * We validate the TempModel which has similar validation as the actual model
     * Dotnet converts empty strings to null => We have to set DepIds, ConIds and RoleIds to empty string on every post (We use them in html to render selection)
     */

    public class CreateModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public CreateModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }


        public List<Role> AvailableRoles;
        public List<ContractType> AvailableContractTypes;
        public List<Department> AvailableDepartments;
        public List<DueDateType> AvailableDueDateTypes;


        // new Task Properties
        [BindProperty] 
        public TaskBlueprintTemp NewTaskBlueprint { get; set; }

        [BindProperty] public string? ToUpdateTaskBlueprintName { get; set; }

        // Needed to know Program stat: IsUpdating == true => UpdateTaskBlueprint validation failed; update button should be displayed
        // IsUpdating == false: We want to create new TaskBlueprint => Display submit button 
        [BindProperty]
        public bool IsUpdating { get; set; }
        
        // ProcessBlueprintSpecific Stuff
        [BindProperty] public string? NewProcessBlueprintName { get; set; }

        [BindProperty] public string? NewProcessBlueprintRoleIds { get; set; }
        public string ErrorMessage { get; set; }


        public List<TaskBlueprintTemp> AllTaskTemp;

        public async Task<IActionResult> OnGetAsync()
        {
            await ReloadLists();
            if (NewProcessBlueprintName == null)
            {
                NewProcessBlueprintName = String.Empty;
            }

            if (NewProcessBlueprintRoleIds == null)
            {
                NewProcessBlueprintRoleIds = String.Empty;
            }
            
            NewTaskBlueprint = GetStaringTemplate();
            TempData.Remove("Tasks");
            AllTaskTemp = new List<TaskBlueprintTemp>();
            IsUpdating = false;
            return Page();
        }

        public async Task<IActionResult> OnGetKeepTemp(string pInstanceName, string? permittedRoleIds)
        {
            await ReloadLists();
            NewTaskBlueprint = GetStaringTemplate();
            NewProcessBlueprintName = pInstanceName;
            NewProcessBlueprintRoleIds = permittedRoleIds;
            NewProcessBlueprintRoleIds ??= string.Empty;
            IsUpdating = false;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateTaskBlueprint()
        {
            NewProcessBlueprintRoleIds ??= string.Empty;
            NewTaskBlueprint.PermittedDepartmentIds ??= string.Empty;
            NewTaskBlueprint.PermittedContractTypeIds ??= string.Empty;
            if (! await CheckTaskBlueprintStateOnCreate(NewTaskBlueprint))
            {
                
                // load lists
                await ReloadLists();
                return Page();
            }

            var tasks = LoadTempData();

            tasks.Add(NewTaskBlueprint);
            TempData["Tasks"] = JsonSerializer.Serialize(tasks);
            
            
            return RedirectToPage("./Create", new { handler = "KeepTemp", pInstanceName = NewProcessBlueprintName, permittedRoleIds = NewProcessBlueprintRoleIds });
        }

        public async Task<IActionResult> OnPostUpdateTaskBlueprint()
        {
           
            if (ToUpdateTaskBlueprintName == null)
            {
                throw new Exception("UpdateTaskId is null. Check js select method!");
            }
            if (! await CheckTaskBlueprintStateOnUpdate(NewTaskBlueprint))
            {
                NewProcessBlueprintRoleIds ??= string.Empty;
                NewTaskBlueprint.PermittedDepartmentIds ??= string.Empty;
                NewTaskBlueprint.PermittedContractTypeIds ??= string.Empty;
                await ReloadLists();
                IsUpdating = true;
                return Page();
            }

            var tasks = LoadTempData();

            var taskToUpdate = tasks.FirstOrDefault(t => t.TaskName == ToUpdateTaskBlueprintName);
            if (taskToUpdate != null)
            {
                DueDateType dType;
                if (!Enum.TryParse(NewTaskBlueprint.DueDateType, out dType))
                {
                    throw new Exception("Parse Error");
                }

                // calculate days
                int newDays;
                if (dType != DueDateType.ASAP && dType != DueDateType.CUSTOM)
                {
                    newDays = DueDateTypeHelper.DueDateTypToDays(dType);
                }
                else
                {
                    // 0 for ASAP & correct day for custom
                    newDays = NewTaskBlueprint.DaysRelativeToDueDate;
                }

                taskToUpdate.DueDateType = NewTaskBlueprint.DueDateType;
                taskToUpdate.TaskName = NewTaskBlueprint.TaskName;
                taskToUpdate.DaysRelativeToDueDate = newDays;
                taskToUpdate.PermittedContractTypeIds = NewTaskBlueprint.PermittedContractTypeIds;
                taskToUpdate.PermittedDepartmentIds = NewTaskBlueprint.PermittedDepartmentIds;
                taskToUpdate.PermittedRoleId = NewTaskBlueprint.PermittedRoleId;
                taskToUpdate.Instructions = NewTaskBlueprint.Instructions;

                TempData["Tasks"] = JsonSerializer.Serialize(tasks);
            }

            IsUpdating = false;
            return RedirectToPage("./Create", new { handler = "KeepTemp", pInstanceName = NewProcessBlueprintName, permittedRoleIds = NewProcessBlueprintRoleIds });
        }


        public IActionResult OnPostDeleteTaskBlueprint()
        {

            if (ToUpdateTaskBlueprintName == null)
            {
                throw new Exception("Cant find TaskBlueprint to Delete");
            }

    
            
            var tasks = LoadTempData();
            var toDel = tasks.FirstOrDefault(t => t.TaskName == ToUpdateTaskBlueprintName);
            if (toDel == null)
            {
                throw new Exception("Cant Delete because task doesnt seem to exist");
            }

            tasks.Remove(toDel);
            TempData["Tasks"] = JsonSerializer.Serialize(tasks);
            return RedirectToPage("./Create", new { handler = "KeepTemp", pInstanceName = NewProcessBlueprintName, permittedRoleIds = NewProcessBlueprintRoleIds });
        }

        public async Task<IActionResult> OnPostCreateProcessBlueprint()
        {
            // First check if PermittedRolesId or Processblueprintname is null or empty
            // if yes: Model is invalid => we don't need to construct ProcessInstance
            ModelState.Clear();
            if (NewProcessBlueprintName == null || NewProcessBlueprintRoleIds == null)
            {
                if (NewProcessBlueprintRoleIds == null) 
                {
                    ModelState.AddModelError("NewProcessBlueprintRoleIds", "Bitte mindestens eine Rolle wählen!");
                    NewProcessBlueprintRoleIds = string.Empty;
                }
                if (NewProcessBlueprintName == null)
                {
                    ModelState.AddModelError("NewProcessBlueprintName", "Bitte einen Namen angeben!");
                    NewProcessBlueprintName = string.Empty;
                }
                // to resolve possible null references (User submits completly empty form)
                // we can set DepIds & ContractTypIds to Empty string
                // shouldn't destroy usability flow, because we only remove selection for task, that shouldn't even 
                // be added to the blueprint
                await ReloadLists();
                NewTaskBlueprint = GetStaringTemplate();
                return Page();
            }
            
            
            var tasks = LoadTempData();
            List<TaskBlueprint> allTasks = new List<TaskBlueprint>();
            foreach (var temp in tasks)
            {
                var permittedDepartments = Department.GetByIds(_context,
                    temp.PermittedDepartmentIds.Split(",").Select(int.Parse).ToList());
                var permittedContractTypes = ContractType.FindByIds(_context,
                    temp.PermittedContractTypeIds.Split(",").Select(int.Parse).ToList());
                DueDateType dDate;
                Enum.TryParse(temp.DueDateType, out dDate);
                TaskBlueprint toAdd = new TaskBlueprint()
                {
                    TaskName = temp.TaskName,
                    DueDateType = dDate,
                    DaysRelativeToDueDate = temp.DaysRelativeToDueDate,
                    Instructions = temp.Instructions,
                    PermittedRole = Role.GetById(_context, temp.PermittedRoleId),
                    PermittedDepartments = permittedDepartments,
                    PermittedContractTypes = permittedContractTypes
                };
                allTasks.Add(toAdd);
            }

            var permittedRoles =
                Role.GetByIds(_context, NewProcessBlueprintRoleIds.Split(",").Select(int.Parse).ToList());
            ProcessBlueprint newBlueprint = new ProcessBlueprint()
            {
                ProcessBlueprintName = NewProcessBlueprintName,
                Tasks = allTasks,
                PermittedRoles = permittedRoles
            };

            if (! await CheckProcessBlueprintState(newBlueprint))
            {
                await ReloadLists();
                NewTaskBlueprint = GetStaringTemplate();
                return Page();
            }
            
            await newBlueprint.CreateAsync(_context);
            TempData.Remove("Tasks");
            return RedirectToPage("Index");
        }

        // check if ModelState of TaskBlueprint is valid on create
        // Error, when TaskBlueprintTemp Validation fails
        // Also Error, when Taskname isnt unique in Templist AND Db
        public async Task<bool> CheckTaskBlueprintStateOnCreate(TaskBlueprintTemp taskBlueprintTemp)
        {
            bool ret = true;
            if (!TryValidateModel(taskBlueprintTemp))
            {
                ret =  false;
                if (NewTaskBlueprint.PermittedContractTypeIds == null) NewTaskBlueprint.PermittedContractTypeIds = string.Empty;
                if (NewTaskBlueprint.PermittedDepartmentIds == null) NewTaskBlueprint.PermittedDepartmentIds = string.Empty;

            }
            
            // no role selected
            // should only be possible, if no roles are present in DB!!!
            if (taskBlueprintTemp.PermittedRoleId == 0)
            {
                ModelState.AddModelError("PermittedRoleId", "Bitte eine Rolle wählen" );
                ret = false;
            }
            // name not unique

            var tasks = LoadTempData();
            if (tasks.FirstOrDefault(t => t.TaskName == NewTaskBlueprint.TaskName) != null)
            {
                ModelState.AddModelError("TaskName","Es soll für diesen Prozess schon eine Aufgabe mit dem Namen '" + NewTaskBlueprint.TaskName + "' erstellt werden!");
                ret = false;
            }

            if (await TaskBlueprint.ExistsAsync(_context, NewTaskBlueprint.TaskName))
            {
                ModelState.AddModelError("TaskName", "Es existiert bereits ein einem anderen Prozess eine Aufgabe mit dem Namen " + NewTaskBlueprint.TaskName + " !");
                ret = false;
            }
            return ret;
        }

        
        // check if ModelState of TaskBlueprint is valid on update
        // Error, when TaskBlueprintTemp Validation fails
        // Error, when newName
        public async Task<bool> CheckTaskBlueprintStateOnUpdate(TaskBlueprintTemp taskBlueprintTemp)
        {
            ModelState.Clear();
            bool ret = true;
            if (!TryValidateModel(taskBlueprintTemp))
            {
                ret =  false;
                if (NewTaskBlueprint.PermittedContractTypeIds == null) NewTaskBlueprint.PermittedContractTypeIds = string.Empty;
                if (NewTaskBlueprint.PermittedDepartmentIds == null) NewTaskBlueprint.PermittedDepartmentIds = string.Empty;

            }
            
            // no role selected
            // should only be possible, if no roles are present in DB!!!
            if (taskBlueprintTemp.PermittedRoleId == 0)
            {
                ModelState.AddModelError("PermittedRoleId", "Bitte eine Rolle wählen" );
                ret = false;
            }
            
            // name not unique
            var tasks = LoadTempData();
            if (tasks.FirstOrDefault(t => t.TaskName == NewTaskBlueprint.TaskName) != null && ToUpdateTaskBlueprintName != NewTaskBlueprint.TaskName)
            {
                ModelState.AddModelError("TaskName","Es soll für diesen Prozess schon eine Aufgabe mit dem Namen '" + NewTaskBlueprint.TaskName + "' erstellt werden!");
                ret = false;
            }

            if (await TaskBlueprint.ExistsAsync(_context, NewTaskBlueprint.TaskName))
            {
                ModelState.AddModelError("TaskName", "Es existiert bereits ein einem anderen Prozess eine Aufgabe mit dem Namen " + NewTaskBlueprint.TaskName + " !");
                ret = false;
            }
            return ret;
        }


        public async Task<bool> CheckProcessBlueprintState(ProcessBlueprint processBlueprint)
        {
            ModelState.Clear();
            bool ret = true;
            ret = TryValidateModel(processBlueprint);
            if (await ProcessBlueprint.ExistsAsync(_context, NewProcessBlueprintName))
            {
                ModelState.AddModelError("NewProcessBlueprintName", "Es exisitert bereits ein Processblueprint mit dem Namen " + NewProcessBlueprintName + " !");
                ret = false;
            }

            if (NewProcessBlueprintName.Length > 25)
            {
                ModelState.AddModelError("NewProcessBlueprintName", "Der Name darf maximal 25 Zeichen lang sein!");
            }
            return ret;
        }
        
        // Create List<TaskBlueprintTemp> from 'TempDate[Tasks]'
        // throws JsonException if TempData cant be serialized as List
        // returns empty list, it TempData is null
        public List<TaskBlueprintTemp> LoadTempData()
        {
            var tasksJson = TempData["Tasks"] as string;
            if (string.IsNullOrEmpty(tasksJson))
            {
                return new List<TaskBlueprintTemp>();
            } 
            var tasks = JsonSerializer.Deserialize<List<TaskBlueprintTemp>>(tasksJson);
            if (tasks == null) throw new JsonException("Temp data couldn't be parsed as List<TaskBlueprintTemp>");
            TempData.Keep("Tasks");
            return tasks;
        }

        
        // reloads AvailableRoles, AvailableContractTypes, AvailableDepartments, AvailableDueDateTypes, AllTaskTemp
        public async Task ReloadLists()
        {
            AvailableRoles =  Role.GetAllRoles(_context);
            AvailableContractTypes = ContractType.GetAllContractTypes(_context);
            AvailableDepartments =  Department.GetAllDepartments(_context);
            AvailableDueDateTypes = DueDateTypeHelper.AllDueDayTypes();
            AllTaskTemp = LoadTempData();
        }

        public TaskBlueprintTemp GetStaringTemplate()
        {
            TaskBlueprintTemp ret = new TaskBlueprintTemp()
            {
                TaskName = String.Empty,
                DueDateType = DueDateType.ASAP.ToString(),
                PermittedRoleId = Role.GetAllRoles(_context).First().Id,
                PermittedDepartmentIds = string.Join(",", AvailableDepartments.Select(d => d.Id)),
                PermittedContractTypeIds = string.Join(",", AvailableContractTypes.Select(c => c.Id))
            };
            return ret;
        }
    }
}