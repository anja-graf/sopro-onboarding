using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;

namespace Replay.Model;

public class TaskInstance
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = ("Aufgabeninstanz muss einen Namen haben. Wenn diese Nachricht angezeigt wird, funktioniert PocessBlueprint validation nicht!!!"))]
    public string? TaskName { get; set; }
    public string? Instructions { get; set; }
    
    public int DaysRelToProcessInstance { get; set; }
    
    // Either Role oder User is selected; never both
    [ValidateNever]
    public User? AssignedUser { get; set; }
    [ValidateNever]
    public Role? AssignedRole { get; set; }
    
    [Required(ErrorMessage = "Aufgabe muss einen Status haben.")]
    public Status TaskStatus { get; set; }

    public bool isAsap { get; set; }
    
    // Foreign key to ProcessInstance
    [Required]
    public int ProcessInstanceId { get; set; }
    
    [ValidateNever]
    public ProcessInstance? ProcessInstance { get; set; }

    public void Remove(ApplicationDbContext _context)
    {
        _context.Remove(this);
        _context.SaveChanges();
    }
    
    public static TaskInstance GetTaskInstanceById(ApplicationDbContext context, int id)
    {
        
        var ret =  context.ProcessInstance
            .Include(p => p.AllTasks)
            .SelectMany(pi => pi.AllTasks)
            .Include(t => t.AssignedRole)
            .Include(t => t.AssignedUser)
            .FirstOrDefault(task => task.Id == id);
        if (ret == null) throw new EntityNotFoundException("TaskInstance: " + id + " wasn't found");
        return ret;

    }
    
    public static void DeleteTaskInstanceById(ApplicationDbContext context, int id)
    {
        var taskToRemove = GetTaskInstanceById(context, id);
        if (taskToRemove != null)
        {
            var process = context.ProcessInstance.FirstOrDefault(p => p.AllTasks.Contains(taskToRemove));
            if (process != null)
            {
                process.AllTasks.Remove(taskToRemove);
                context.SaveChanges();
            }
        }
    }
    
    public static void UpdateTaskInstanceById(ApplicationDbContext context,int taskId, string taskName, int distanceToDueDate, User? assignedUser, Role? assignedRole, Status newStatus, string instructions)
    {
        var task = GetTaskInstanceById(context, taskId);

        if (task != null)
        {
            task.TaskName = taskName;
            task.Instructions = instructions;
            /*either role or user is not null*/
            if (assignedUser != null)
                task.AssignedUser = assignedUser;
            else
            {
                task.AssignedRole = assignedRole;
                task.AssignedUser = null;
            }
            task.TaskStatus = newStatus;
            context.SaveChanges();
        }
    }

    public static void PickTaskInstanceById(ApplicationDbContext context, int taskId, User assignedUser)
    {
        var task = GetTaskInstanceById(context, taskId);

        if (task != null)
        {
            task.AssignedUser = assignedUser;
            context.SaveChanges();
        }
    }

    public static void CreateTaskInstance(ApplicationDbContext context, int processInstanceId, string taskName, int distanceToDueDate, User? assignedUser,
        Role? assignedRole, Status status, string instructions,bool isAsap)
    {
        TaskInstance newTask = new TaskInstance();
        newTask.TaskName = taskName;
        newTask.Instructions = instructions;
        //newTask.DueDate = duedate;
        /*either role or user is not null*/
        if (assignedUser != null)
            newTask.AssignedUser = assignedUser;
        else
        {
            newTask.AssignedRole = assignedRole;
            newTask.AssignedUser = null;
        }
        newTask.TaskStatus = status;
        var pInstance = context.ProcessInstance.Include(p => p.AllTasks).FirstOrDefault(p => p.Id == processInstanceId);
        if (pInstance != null)
        {
            pInstance.AllTasks.Add(newTask);
            context.SaveChanges();
        }
    }

    public static int CalculateDaysToToday(DateOnly dueDate)
    {
        Console.WriteLine("Distanz zwischen " + dueDate.ToString() + " und heute " + (DateOnly.FromDateTime(DateTime.Today).DayNumber - dueDate.DayNumber) + " Tage");
        return dueDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber ;
    }

    public static string StringOutOfDays(int days)
    {
        string s = "";
        if (days == 0) return "heute";
        if (days < 0)
        {
            s += "vor ";
            days *= -1;
        }
        else s += "in ";

        s += days + " Tag";
        if (days != 1)
            s += "en";
        return s;
    }

    public int CalculateDays(ApplicationDbContext context, DateOnly dueDate)
    {
        var process = this.GetProcessInstance(context);
        if (process != null)
        {
            Console.WriteLine(process.DueDate.ToString());
            return process.DueDate.DayNumber - dueDate.DayNumber;
        }
        throw new Exception();
    }
    
    public static int CalculateDays(ProcessInstance process, DateOnly dueDate)
    {
        Console.WriteLine(process.DueDate.ToString());
        return process.DueDate.DayNumber - dueDate.DayNumber;
    }
    public DateOnly CalculateDate(ApplicationDbContext context, int days)
    {
        var process = GetProcessInstance(context);
        if (process != null)
        {
            return AddDaysTo(process.DueDate, -days);
        }

        throw new Exception();
    }

    public static DateOnly AddDaysTo(DateOnly dueDate, int days)
    {
        return dueDate.AddDays(days);
    }

    private ProcessInstance? GetProcessInstance(ApplicationDbContext context)
    {
        return context.ProcessInstance.Include(p => p.AllTasks).FirstOrDefault(p => p.Id == this.ProcessInstanceId);
    }
    
    public static ProcessInstance? GetProcessInstanceById(ApplicationDbContext context, int id)
    {
        return context.ProcessInstance.Include(p => p.AllTasks).FirstOrDefault(p => p.Id == id);
    }
}