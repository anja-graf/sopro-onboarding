
using System.ComponentModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Model;

namespace Replay.Pages.ProcessBlueprintViews
{
    public class Editmodel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public Editmodel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }


        public List<Role> AvailableRoles;
        public List<ContractType> AvailableContractTypes;
        public List<Department> AvailableDepartments;
        public List<DueDateType> AvailableDueDateTypes;
        public List<TaskBlueprint> AllTasks;


        // new Task Properties
        [BindProperty] public TaskBlueprintTemp NewTaskBlueprint { get; set; }

        [BindProperty] public string? ToUpdateTaskBlueprintName { get; set; }

        // Needed to know Program stat: IsUpdating == true => UpdateTaskBlueprint validation failed; update button should be displayed
        // IsUpdating == false: We want to create new TaskBlueprint => Display submit button 
        [BindProperty] public bool IsUpdating { get; set; }

        // ProcessBlueprintSpecific Stuff

        [BindProperty] public string? NewProcessBlueprintRoleIds { get; set; }

        [BindProperty]
        public string ToEditProcessBlueprintName { get; set; }
        public ProcessBlueprint ToEditProcessBlueprint { get; set; }


        public async Task<IActionResult> OnGetAsync(string toEdit)
        {
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, toEdit);
            NewProcessBlueprintRoleIds = string.Join(",", ToEditProcessBlueprint.PermittedRoles.Select(r => r.Id));
            ReloadLists();
            NewTaskBlueprint = GetStaringTemplate();
            return Page();
        }

        public async Task<IActionResult> OnGetKeepSelections(string toEdit, string selectedRoles)
        {
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, toEdit);
            NewProcessBlueprintRoleIds = selectedRoles;
            NewProcessBlueprintRoleIds ??= string.Empty;
            ReloadLists();
            NewTaskBlueprint = GetStaringTemplate();
            IsUpdating = false;
            return Page();
        }
        
        public async Task<IActionResult> OnPostCreateTaskBlueprint()
        {
            NewTaskBlueprint.PermittedDepartmentIds ??= string.Empty;
            NewTaskBlueprint.PermittedContractTypeIds ??= string.Empty;
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, ToEditProcessBlueprintName);
            if (! await CheckTaskBlueprintStateOnCreate(NewTaskBlueprint))
            {
                
                ReloadLists();
                return Page();
            }
            
            // save NewTaskBlueprint
            DueDateType dDate;
            if (!Enum.TryParse(NewTaskBlueprint.DueDateType, out dDate))
                throw new InvalidEnumArgumentException(NewTaskBlueprint.DueDateType + " cant be parsed as DueDateType");
            List<Department> permittedDepartments = Department.GetByIds(_context,
                NewTaskBlueprint.PermittedDepartmentIds.Split(",").Select(int.Parse).ToList());
            List<ContractType> permittedContractTypes = ContractType.FindByIds(_context,
                NewTaskBlueprint.PermittedContractTypeIds.Split(",").Select(int.Parse).ToList());
            TaskBlueprint toSave = new TaskBlueprint()
            {
                TaskName = NewTaskBlueprint.TaskName,
                DueDateType = dDate,
                DaysRelativeToDueDate = NewTaskBlueprint.DaysRelativeToDueDate,
                Instructions = NewTaskBlueprint.Instructions,
                PermittedRole = Role.GetById(_context, NewTaskBlueprint.PermittedRoleId),
                PermittedDepartments = permittedDepartments,
                PermittedContractTypes = permittedContractTypes,
                ProcessBlueprint = ToEditProcessBlueprint
            };
            if (!TryValidateModel(toSave)) throw new Exception("TaskBlueprintTemp validation seems to be wrong! TaskBlueprint is not valid!!");
            await toSave.SaveAsync(_context);
            return RedirectToPage("./Edit",new { handler ="KeepSelections", toEdit = ToEditProcessBlueprint.ProcessBlueprintName, selectedRoles = NewProcessBlueprintRoleIds });
        }
        
        public async Task<IActionResult> OnPostUpdateTaskBlueprint()
        {
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context ,ToEditProcessBlueprintName);
            if (ToUpdateTaskBlueprintName == null)
            {
                throw new Exception("UpdateTaskId is null. Check js select method!");
            }

            if (! await CheckTaskBlueprintStateOnUpdate(NewTaskBlueprint))
            {
                IsUpdating = true;
                ReloadLists();
                return Page();
            }
            
            
            // Find Task in DB & Update
            TaskBlueprint toEdit = await TaskBlueprint.FindByIdAsync(_context, ToUpdateTaskBlueprintName);
            
            //EFCore: Primary Key cant be updated => delete old instance & create new
            await toEdit.RemoveAsync(_context);
            
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
            List<Department> permittedDepartments = Department.GetByIds(_context,
                NewTaskBlueprint.PermittedDepartmentIds.Split(",").Select(int.Parse).ToList());
            List<ContractType> permittedContractTypes = ContractType.FindByIds(_context,
                NewTaskBlueprint.PermittedContractTypeIds.Split(",").Select(int.Parse).ToList());
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, ToEditProcessBlueprintName);
            toEdit.DueDateType = dType;
            toEdit.TaskName = NewTaskBlueprint.TaskName;
            toEdit.DaysRelativeToDueDate = newDays;
            toEdit.PermittedContractTypes = permittedContractTypes;
            toEdit.PermittedDepartments = permittedDepartments;
            toEdit.PermittedRoleId = NewTaskBlueprint.PermittedRoleId;
            toEdit.Instructions = NewTaskBlueprint.Instructions;
            toEdit.ProcessBlueprint = ToEditProcessBlueprint;

            if (!TryValidateModel(toEdit)) throw new Exception("TaskBlueprintTemp validation seems to be wrong! TaskBlueprint is not valid!!");
            await toEdit.SaveAsync(_context);
            IsUpdating = false;
            return RedirectToPage("./Edit",new { handler ="KeepSelections", toEdit = ToEditProcessBlueprint.ProcessBlueprintName, selectedRoles = NewProcessBlueprintRoleIds });
        }

        public async Task<IActionResult> OnPostDeleteTaskBlueprint()
        {

            if (ToUpdateTaskBlueprintName == null)
            {
                throw new Exception("Cant find TaskBlueprint to Delete");
            }
            
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, ToEditProcessBlueprintName);
            
            var toDel = ToEditProcessBlueprint.Tasks.FirstOrDefault(t => t.TaskName == ToUpdateTaskBlueprintName);
            if (toDel == null)
            {
                throw new Exception("Cant Delete because task doesnt seem to exist");
            }

            await toDel.RemoveAsync(_context);
            
            return RedirectToPage("./Edit",new { handle="KeepSelections", toEdit = ToEditProcessBlueprint.ProcessBlueprintName, selectedRoles = NewProcessBlueprintRoleIds });
        }
        
        public async Task<IActionResult> OnPostCreateProcessBlueprint()
        {
            bool vaild = true;
            ToEditProcessBlueprint = await ProcessBlueprint.FindByNameAsync(_context, ToEditProcessBlueprintName);
            ModelState.Clear();
            if (NewProcessBlueprintRoleIds == null)
            {
                vaild = false;
                ModelState.AddModelError("NewProcessBlueprintRoleIds", "Bitte mindestens eine Rolle wählen!");
                NewProcessBlueprintRoleIds = string.Empty;
            }
            // update Processblueprint
            if (!string.IsNullOrEmpty(NewProcessBlueprintRoleIds))
            {
                ToEditProcessBlueprint.PermittedRoles = Role.GetByIds(_context, NewProcessBlueprintRoleIds.Split(",").Select(int.Parse).ToList());
                if (!TryValidateModel(ToEditProcessBlueprint))
                {
                    vaild = false;
                }
            }

            if (vaild == false)
            {
                ReloadLists();
                NewTaskBlueprint = GetStaringTemplate();
                return Page();
            }
            await ToEditProcessBlueprint.UpdateAsync(_context);
            return RedirectToPage("Index");
        }
        
        public async Task<bool> CheckTaskBlueprintStateOnCreate(TaskBlueprintTemp taskBlueprintTemp)
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
            
            var tasks = ToEditProcessBlueprint.Tasks;
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
        
        public async Task<bool> CheckTaskBlueprintStateOnUpdate(TaskBlueprintTemp newTaskBlueprint)
        {
            bool ret = true;
            ModelState.Clear();
            if (!TryValidateModel(newTaskBlueprint))
            {
                ret = false;
                NewTaskBlueprint.PermittedDepartmentIds ??= string.Empty;
                NewTaskBlueprint.PermittedContractTypeIds ??= string.Empty;
            }

            // Role id == 0 => no role selected
            if (NewTaskBlueprint.PermittedRoleId == 0)
            {
                ModelState.AddModelError("PermittedRoleId", "Bitte eine Rolle wählen" );
                ret = false;
            }
            
            // is new name already in db? 
            if (await TaskBlueprint.ExistsAsync(_context, NewTaskBlueprint.TaskName) && NewTaskBlueprint.TaskName != ToUpdateTaskBlueprintName)
            {
                ModelState.AddModelError("TaskName", "Es existiert bereits ein einem anderen Prozess eine Aufgabe mit dem Namen " + NewTaskBlueprint.TaskName + " !");
                ret = false;
            }

            return ret;

        }

        public bool CheckProcessBlueprintState(ProcessBlueprint processBlueprint)
        {
            ModelState.Clear();
            bool ret = true;
            if (NewProcessBlueprintRoleIds == null)
            {
                ModelState.AddModelError("NewProcessBlueprintRoleIds", "Bitte mindestens eine Rolle wählen!");
                NewProcessBlueprintRoleIds = string.Empty;
            }
            
            return ret;
        }
        

        // reloads AvailableRoles, AvailableContractTypes, AvailableDepartments, AvailableDueDateTypes, AllTaskTemp
        public void ReloadLists()
        {
            AvailableRoles = Role.GetAllRoles(_context);
            AvailableContractTypes = ContractType.GetAllContractTypes(_context);
            AvailableDepartments = Department.GetAllDepartments(_context);
            AvailableDueDateTypes = DueDateTypeHelper.AllDueDayTypes();
            AllTasks = ToEditProcessBlueprint.Tasks;
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