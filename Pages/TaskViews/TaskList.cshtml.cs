using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Data;
using Replay.Model;


namespace Replay.Pages.TaskViews;

public class TaskList : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly string _demoUserEmail = "demo@abc.de";


    
    public TaskList(ApplicationDbContext context)
    {
        _context = context;
    } 
    
    public void OnGet()
    {
        
        AllTasks = GetTasksForUser(_demoUserEmail);

    }

    public IActionResult OnPostUpdateTaskStatus(int taskId, Status newStatus)
    {
        // find task in instances
        Console.WriteLine(taskId + " " + newStatus);
        TaskInstance toEdit = null;
        foreach (var pInstance in MockData.AllInstances)
        {
            foreach (var t in pInstance.AllTasks)
            {
                if (t.Id == taskId)
                {
                    toEdit = t;
                }
            }
        }
        //ToDO: Dont user demo user here
        if (newStatus == Status.IN_PROGRESS)
        {
            toEdit.AssignedUser = MockData.demo;
        }
        toEdit.TaskStatus = newStatus;
        AllTasks = GetTasksForUser(_demoUserEmail);
        return RedirectToPage();
    }
    
    
    

   
    public List<TaskListTaskViewModel> AllTasks { get; set; } = new List<TaskListTaskViewModel>();
    
    
    // Todo: Make work with identity & Roles
    private List<TaskListTaskViewModel> GetTasksForUser(string usermail)
    {
        List<TaskListTaskViewModel> ret = new List<TaskListTaskViewModel>();
        foreach (var pInstance in MockData.AllInstances)
        {
            foreach (var task in pInstance.AllTasks)
            {
                if (task.AssignedUser != null && task.AssignedUser.Email == usermail)
                {
                    TaskListTaskViewModel toAdd = new TaskListTaskViewModel()
                    {
                        Id = task.Id,
                        ProcessName = pInstance.ProcessInstanceName,
                        TaskName = task.TaskName,
                        ResponsibleUserName = pInstance.ResponsibleUser.Name,
                        ReferenceUserName = pInstance.ReferenceUser.Name,
                        //DueDate = task.DueDate,
                        Status = task.TaskStatus,
                        Instructions = task.Instructions
                    };
                    ret.Add(toAdd);
                }
                // Assigned Roles shoudnlt be null if we get here
                
                if (task.AssignedRole != null /*&&  MockData.demo.Roles.Intersect(task.AssignedRole).Any() */)
                {
                    TaskListTaskViewModel toAdd = new TaskListTaskViewModel()
                    {
                        Id = task.Id,
                        ProcessName = pInstance.ProcessInstanceName,
                        TaskName = task.TaskName,
                        ResponsibleUserName = pInstance.ResponsibleUser.Name,
                        ReferenceUserName = pInstance.ReferenceUser.Name,
                        //DueDate = task.DueDate,
                        Status = task.TaskStatus,
                        Instructions = task.Instructions
                    };
                    ret.Add(toAdd);
                }                
                //  Debug stuff
                if (task.AssignedUser == null && (task.AssignedRole == null))
                {
                    Console.WriteLine("Error: AssignedUser and AssignedRoles are BOTH null, this schouldn't be possible ");
                }
            }
        }

        return ret;
    }
}