using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.TaskViews;

public class TaskCreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TaskCreateModel(ApplicationDbContext context)
    {
        _context = context;
        AllUser = _context.User.ToList();
        AllRoles = _context.Role.ToList();
        AllStatus = Enum.GetValues(typeof(Status))
            .Cast<Status>()
            .ToList();
    }

    public IActionResult OnGet(int processInstanceId)
    {
        DisplayTask = new TaskInstance();
        DisplayTask.TaskStatus = Status.NOT_STARTED;
        DisplayTask.TaskName = "";
        DisplayTask.AssignedRole = _context.Role.ToList().First();
        ProcessInstanceId = processInstanceId;
        AllUser = _context.User.ToList();
        AllRoles = _context.Role.ToList();
        AllStatus = Enum.GetValues(typeof(Status))
            .Cast<Status>()
            .ToList();
        TaskDueDate = DateOnly.FromDateTime(DateTime.Today);
        return Page();
    }

    [BindProperty]
    public TaskInstance DisplayTask { get; set; } = default!;
    public int ProcessInstanceId { get; set; }
    public DateOnly TaskDueDate { get; set; }
    public List<User> AllUser { get; set; }
    public List<Role> AllRoles { get; set; }
    public List<Status> AllStatus { get; set; }

    
    public IActionResult OnPostSave(string taskNameInput, string? dueDateInput,DueDateType dueDateTypeInput,
        Status taskStatus, string instructions, string? responsible)
    {
        if (string.IsNullOrEmpty(instructions)) instructions = " ";

        Role? responsibleRole = AllRoles.Find(r => r.RoleName == responsible);
        User? responsibleUser = null;
        if (responsibleRole == null) responsibleUser = AllUser.Find(r => r.Name ==responsible);

        var isAsap = (dueDateTypeInput == DueDateType.ASAP) ? true : false;
        int days = 0;

        var process = TaskInstance.GetProcessInstanceById(_context, ProcessInstanceId);
        if (dueDateTypeInput == DueDateType.CUSTOM)
            days = TaskInstance.CalculateDays(process, DateOnly.Parse(dueDateInput));
        else if (dueDateTypeInput != DueDateType.ASAP)
            days = DueDateTypeHelper.DueDateTypToDays(dueDateTypeInput) *-1;
        Console.WriteLine(days);
        
        //int days = DisplayTask.CalculateDays(_context, DateOnly.Parse(dueDateInput));
        TaskInstance.CreateTaskInstance(_context,ProcessInstanceId,taskNameInput,days,responsibleUser,responsibleRole,taskStatus,instructions,isAsap);
        //return Page();
        return RedirectToPage("Edit");
    }
    
}