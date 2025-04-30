using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Migrations;
using Replay.Model;
using Replay.Pages.TaskViews;


namespace Replay.Pages.TaskViews;

public class TaskDetailModel : PageModel
{
    private readonly ApplicationDbContext _context;
    
    [BindProperty]
    public TaskInstance DisplayTask { get; set; }
    public DateOnly TaskDueDate { get; set; }
    public TaskDetailModel(ApplicationDbContext context) => _context = context;
    public IActionResult OnGet(int taskId)
    {
        var temp = TaskInstance.GetTaskInstanceById(_context, taskId);
        if (temp == null)
        {
            Console.WriteLine("Task konnte nicht gefunden werden");
            return NotFound();
        }
        DisplayTask = temp;
        DisplayTask.TaskName = temp.TaskName;
        //DisplayTask.DueDate = temp.DueDate;
        DisplayTask.TaskStatus = temp.TaskStatus;
        DisplayTask.Instructions = temp.Instructions;
        return Page();
    }
/*
    public IActionResult OnPost(string DueDateInput,string UserName, string action)
    {
        if (DisplayTask != null)
        {
            Console.WriteLine(DisplayTask.Id);
            var task = MockData.getTaskInstanceByID(DisplayTask.Id);
            if (task == null)
                return NotFound();
            switch (action)
            {
                case "Mir zuweisen":
                    //ToDo aktueller Benutzer noch nicht bekannt 
                    // Quickfix with demo user
                    // toDo: This also puts Task with Status = IN_PRogress to IN_PROGRESS
                    MockData.UpdateTaskInstanceByID(task.Id, MockData.demo, null, Status.IN_PROGRESS);
                    return RedirectToPage("TaskList"); //return Page();
                case "Löschen":
                    MockData.DeleteTaskInstanceByID(task.Id);
                    return RedirectToPage("TaskList");
            }
        }
        return NotFound();
    }*/


    public IActionResult OnPostDelete(int taskId)
    {
        if (TaskInstance.GetTaskInstanceById(_context,taskId) == null)
        {
            Console.WriteLine("Trying to Delete non existent: " + taskId);
            return NotFound();
        }
        Console.WriteLine("Should Delete: " + taskId);
        TaskInstance.DeleteTaskInstanceById(_context,taskId);
        return RedirectToPage("TaskList");

    }

    public IActionResult OnPostUpdateTaskStatus(int taskId, Status newStatus)
    {
        Console.WriteLine("Update: " + taskId+ newStatus);
        var tInstance = TaskInstance.GetTaskInstanceById(_context, taskId);
        if (tInstance == null)
        {
            Console.WriteLine("Trying to Update non existent: " + taskId);
            return NotFound();
        }
        tInstance.TaskStatus = newStatus;
        // Todo: Use real User and not Demo User 
        tInstance.AssignedRole = null;
        tInstance.AssignedUser = MockData.demo;
        return RedirectToPage("TaskList");
    }
}