using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Replay.Data;
using Replay.Model;


namespace Replay.Pages.TaskViews;

public class TaskEditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TaskEditModel(ApplicationDbContext context)
    {
        _context = context;
        AllUser = _context.User.ToList();
        AllRoles = _context.Role.ToList();
        AllStatus = Enum.GetValues(typeof(Status))
            .Cast<Status>()
            .ToList();
    }
    public IActionResult OnGet(int taskId)
    {
        var temp = TaskInstance.GetTaskInstanceById(_context,taskId);
        if (temp == null)
        {
            return NotFound();
        }

        DisplayTask = temp;
        DisplayTask.isAsap = temp.isAsap;
        TaskDueDate = (temp.isAsap == false) ? temp.CalculateDate(_context,temp.DaysRelToProcessInstance) : TaskDueDate = DateOnly.FromDateTime(DateTime.Today);
        AllUser = _context.User.ToList();
        AllRoles = _context.Role.ToList();
        AllStatus = Enum.GetValues(typeof(Status))
            .Cast<Status>()
            .ToList();
        return Page();
    }
    

    [BindProperty] public TaskInstance DisplayTask { get; set; }
    public List<User> AllUser { get; set; }
    public List<Role> AllRoles { get; set; }
    public List<Status> AllStatus { get; set; }
    public DateOnly TaskDueDate { get; set; }

    /*
    public IActionResult OnPost(string DueDateInput, string UserName, string action)
    {
        if (DisplayTask != null)
        {
            var task = TaskInstance.GetTaskInstanceById(_context,DisplayTask.Id);
            if (task == null)
                return NotFound();
            switch (action)
            {
                case "Speichern":
                    //DisplayTask.DueDate = DateOnly.Parse(DueDateInput);
                    foreach (var user in MockData.GetAllUser())
                    {
                        if (user.Name == UserName)
                        {
                            DisplayTask.AssignedUser = user;
                        }
                    }

                    task.TaskName = DisplayTask.TaskName;
                    //task.DueDate = DisplayTask.DueDate;
                    task.TaskStatus = DisplayTask.TaskStatus;
                    task.Instructions = DisplayTask.Instructions;
                    return RedirectToPage("Details", task.Id);
                case "Löschen":
                    TaskInstance.DeleteTaskInstanceById(_context,task.Id);
                    //MockData.DeleteTaskInstanceByID(task.Id);
                    return RedirectToPage("TaskList");
            }
        }

        return NotFound();
    }
*/
    public IActionResult OnPostSave(int taskId, string taskNameInput, string? dueDateInput, DueDateType dueDateTypeInput,
        Status taskStatus, string instructions, string? responsible)
    {
        TaskInstance? task;
        if ((task = TaskInstance.GetTaskInstanceById(_context,taskId)) == null || responsible == null)
        {
            Console.WriteLine("Want to modify non existing Task: "  + taskId);
            return NotFound();
        }

        if (string.IsNullOrEmpty(instructions)) instructions = " ";

        Role? responsibleRole = AllRoles.Find(r => r.RoleName == responsible);
        User? responsibleUser = null;
        if (responsibleRole == null) responsibleUser = AllUser.Find(r => r.Name ==responsible);
        
        if (dueDateTypeInput == DueDateType.ASAP) task.isAsap = true;
        else task.isAsap = false;
        int days = 0;
        if (dueDateTypeInput == DueDateType.CUSTOM)
            days = task.CalculateDays(_context, DateOnly.Parse(dueDateInput));
        else if (dueDateTypeInput != DueDateType.ASAP)
            days = DueDateTypeHelper.DueDateTypToDays(dueDateTypeInput) *-1;
        Console.WriteLine(days);
        TaskInstance.UpdateTaskInstanceById(_context,taskId,taskNameInput,days,responsibleUser,responsibleRole,taskStatus,instructions);
        return RedirectToPage("Details", new {taskId});
    }

    public IActionResult OnPostDelete(int taskId)
    {
        if (TaskInstance.GetTaskInstanceById(_context,taskId) == null)
        {
            Console.WriteLine("Want to delete non existing Task: " + taskId);
            return NotFound();
        }

        TaskInstance.DeleteTaskInstanceById(_context, taskId);
        //MockData.DeleteTaskInstanceByID(taskId);
        return RedirectToPage("TaskList");
    }
}