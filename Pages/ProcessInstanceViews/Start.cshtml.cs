using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Data;
using Replay.Exceptions;
using Replay.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Replay.Pages.ProcessInstanceViews;

public class Start : PageModel
{
    private readonly ApplicationDbContext _context;
    public ProcessBlueprint ToInsanceBlueprint;
    public List<ContractType> AllContractTypes;
    public List<Department> AllDepartments;
    public List<User> AllUsers;
    
    // Bind Properties
    [BindProperty]
    public string ProcessBlueprintName { get; set; }
    
    [Required(ErrorMessage = "E-Mail Feld darf nicht leer sein!")]
    [BindProperty]
    public string RespUserMail { get; set; }
    [Required(ErrorMessage = "E-Mail Feld darf nicht leer sein!")]
    [BindProperty]
    public string RefUserMail { get; set; }
    
    // has to be string, becaus DateOnly cant be bound (DateOnly doesnt have a constructor without parameters)
    [Required(ErrorMessage = "Datum Feld darf nicht leer sein!")]
    [BindProperty]
    public string PInstanceDueDate { get; set; }
    [BindProperty]
    public int ContractId { get; set; }
    [BindProperty]
    public int DepartmentId { get; set; }
    public Start(ApplicationDbContext context) => _context = context;
    
    public async Task<IActionResult> OnGetAsync(string toCreate)
    {
        ToInsanceBlueprint = await ProcessBlueprint.FindByNameAsync(_context, toCreate);
        AllContractTypes = ContractType.GetAllContractTypes(_context);
        AllDepartments = Department.GetAllDepartments(_context);
        AllUsers = Model.User.GetAll(_context);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateInstanceAsync()
    {
        // create TaskInstances for ProcessBlueprint TaskBlueprints
        List<TaskInstance> taskInstances = new List<TaskInstance>();
        ToInsanceBlueprint = await ProcessBlueprint.FindByNameAsync(_context, ProcessBlueprintName);
        ContractType? tInstancesContract = null;
        Department? tInstanceDepartment = null;
        User? refUser = null;
        User? respUser = null;
        DateOnly? DueDate = null;
        try
        { 
            tInstancesContract = ContractType.FindById(_context, ContractId);
        } catch (EntityNotFoundException e)
        {
            ModelState.AddModelError("ContractId", "Bitte eine gültige Vertragsart wählen");
        }

        try
        {
            tInstanceDepartment = Department.GetById(_context, DepartmentId);
        }
        catch (EntityNotFoundException)
        {
            ModelState.AddModelError("DepartmentId", "Bitte gültige Abteilung wählen");
        }

        try
        {
            refUser = Model.User.LoadUser(_context, RefUserMail);
        }
        catch (EntityNotFoundException)
        {
            ModelState.AddModelError("RefUserMail", "User mit angegebener Email nicht gefunden. Bitte existierende Email angeben!");
        }

        try
        {
            respUser = Model.User.LoadUser(_context, RespUserMail);
        }
        catch (EntityNotFoundException)
        {
            ModelState.AddModelError("RespUserMail", "User mit angegebener Email nicht gefunden. Bitte existierende Email angeben!");
        }

        try
        {
            DueDate = DateOnly.Parse(PInstanceDueDate);
        }
        catch (ArgumentNullException)
        {
            ModelState.AddModelError("PInstanceDueDate", "Bitte ein gültiges Datum eingeben");
        }

        if (ModelState.ErrorCount > 0)
        {
            await OnGetAsync(ProcessBlueprintName);
            return Page();    
        }
        
        foreach (var tBlueprint in ToInsanceBlueprint.Tasks)
        {
            User assignedUser = null;
            Status taskStatus = Status.NOT_STARTED;
            
            // does Task have Role 'Bezugsperson' or 'Vorgangsverantwortlicher' ?
            // if yes assign user and set status to Started (no one else shoud pick this task; assigend user already picked it)
            if (Role.GetByName(_context, "Bezugsperson") == tBlueprint.PermittedRole)
            {
                assignedUser = refUser;
                taskStatus = Status.IN_PROGRESS;
            }
            else if (Role.GetByName(_context, "Vorgangsverantwortlicher") == tBlueprint.PermittedRole)
            {
                assignedUser = respUser;
                taskStatus = Status.IN_PROGRESS;
            }
          
            if (tBlueprint.PermittedContractTypes.Select(ct => ct.Id).Contains(tInstancesContract.Id) &&
                tBlueprint.PermittedDepartments.Select(dep => dep.Id).Contains(tInstanceDepartment.Id))
            {
                TaskInstance newInstance = new TaskInstance()
                {
                    TaskName = tBlueprint.TaskName,
                    Instructions = tBlueprint.Instructions,
                    AssignedRole = tBlueprint.PermittedRole,
                    AssignedUser = assignedUser,
                    DaysRelToProcessInstance = tBlueprint.DaysRelativeToDueDate,
                    TaskStatus = taskStatus
                };
                ModelState.Clear();
                if (!TryValidateModel(newInstance))
                {
                    var validationErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = validationErrors });
                }
                taskInstances.Add(newInstance);
            }
        }
        
        
        // Now create ProcessInstance
        ProcessInstance processInstance = new ProcessInstance()
        {
            ProcessInstanceName = ProcessBlueprintName,
            DueDate = DateOnly.Parse(PInstanceDueDate),
            Contract = tInstancesContract,
            Department = tInstanceDepartment,
            ResponsibleUser = respUser,
            ReferenceUser = refUser,

            AllTasks = taskInstances,
            IsArchived = false,

        };

        ModelState.Clear();
        /*if (!TryValidateModel(processInstance)) 
        {
            
            var validationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { Errors = validationErrors });
        }
        */
        Console.WriteLine("Neue Tasks: " +  taskInstances.Count);
        await processInstance.CreateAsync(_context);
        return RedirectToPage("Index");

    }
}