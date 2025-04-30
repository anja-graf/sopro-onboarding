using Replay.Model;

namespace Replay.Pages.TaskViews;

public class MockData
{
    public static readonly User max = new User()
    {
        Email = "max@abc.de",
        Name = "Max Mustermann"
    };

    public static readonly User jane = new User()
    {
        Email = "jane@abc.de",
        Name = "Jane Doe"
    };

    public static Role Personal = new Role()
    {
        Id = 0,
        RoleName = "Personal"
    };

    public static Role Backoffice = new Role()
    {
        Id = 1,
        RoleName = "Backoffice"
    };
    
    public static readonly User demo = new User()
    {
        Email = "demo@abc.de",
        Name = "Admin User",
        Roles = new List<Role>() {Personal, Backoffice}
    };

    public static List<User> AllUser { get; set; } = new List<User>();

    public static List<User> GetAllUser()
    {
        AllUser.Add(max);
        AllUser.Add(demo);
        AllUser.Add(jane);
        return AllUser;
    }
    // Note:    When TaskInstance is assigned to Role: Status = NOT_STARTED
    //          When TaskInstance is assigned to specific User: Status = IN_PROGRESS
     public static List<ProcessInstance> AllInstances { get; set; } = new List<ProcessInstance>()
    {
        new ProcessInstance()
        {
            ProcessInstanceName = "Onboarding",
            DueDate = new DateOnly(2024, 12, 12),
            IsArchived = false,
            ReferenceUser = max,
            ResponsibleUser = jane,
            
            AllTasks = new List<TaskInstance>()
            {
                new TaskInstance()
                {
                    Id = 0,
                    TaskName = "Rahmenbedingungen",
                    //DueDate = new DateOnly(2024, 12, 12),
                    Instructions = "Test",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 1,
                    TaskName = "Weitergabe der Informationen",
                    //DueDate = new DateOnly(2024, 12, 31),
                    Instructions = "Test 2",
                    AssignedUser =demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 2,
                    TaskName = "Preboarding",
                    //DueDate = new DateOnly(2024, 12, 12),
                    Instructions = "Test 3",
                    AssignedRole =  Backoffice  
                },
                new TaskInstance()
                {
                    Id = 3,
                    TaskName = "Organisatorisches vor Start",
                    //DueDate = new DateOnly(2024, 12, 12),
                    Instructions = "Test 3",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 4,
                    TaskName = "Antritt der Stelle",
                    //DueDate = new DateOnly(2024, 12, 12),
                    Instructions = "Test 3",
                    AssignedRole = Personal  
                },
                new TaskInstance()
                {
                    Id = 5,
                    TaskName = "Feedbackrunde",
                    //DueDate = new DateOnly(2024, 12, 12),
                    Instructions = "Test 3",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
            }
        },
        new ProcessInstance()
        {
            ProcessInstanceName = "Onboarding",
            DueDate = new DateOnly(2024, 12, 22),
            IsArchived = false,
            ReferenceUser = max,
            ResponsibleUser = max,
            
            AllTasks = new List<TaskInstance>()
            {
                new TaskInstance()
                {
                    Id = 6,
                    TaskName = "Rahmenbedingungen",
                    //DueDate = new DateOnly(2024, 12, 22),
                    Instructions = "Test",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 7,
                    TaskName = "Weitergabe der Informationen",
                    //DueDate = new DateOnly(2024, 12, 22),
                    Instructions = "Test 2",
                    AssignedUser =demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 8,
                    TaskName = "Preboarding",
                    //DueDate = new DateOnly(2024, 12, 22),
                    Instructions = "Test 3",
                    AssignedRole = Personal 
                },
                new TaskInstance()
                {
                    Id = 9,
                    TaskName = "Organisatorisches vor Start",
                    //DueDate = new DateOnly(2024, 12, 22),
                    Instructions = "Test 3",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
                new TaskInstance()
                {
                    Id = 10,
                    TaskName = "Antritt der Stelle",
                    //DueDate = new DateOnly(2024, 12, 22),
                    Instructions = "Test 3",
                    AssignedRole = Personal
                },
                new TaskInstance()
                {
                    Id = 11,
                    TaskName = "Feedbackrunde",
                    //DueDate = new DateOnly(2024, 12, 25),
                    Instructions = "Test 3",
                    AssignedUser = demo,
                    TaskStatus = Status.IN_PROGRESS
                },
            }
        },
    };

    public static TaskInstance? getTaskInstanceByID(int id)
    {
        foreach (var pInstance in MockData.AllInstances)
        {
            foreach (var task in pInstance.AllTasks)
            {
                if (task.Id == id)
                {
                    return task;
                }
            }
        }

        return null;
    }
    
    
    public static void DeleteTaskInstanceByID(int id)
    {
        foreach (var pInstance in MockData.AllInstances)
        {
            foreach (var task in pInstance.AllTasks)
            {
                if (task.Id == id)
                {
                    pInstance.AllTasks.Remove(task);
                    return;
                }
            }
        }
    }

    public static void UpdateTaskInstanceByID(int id, User assignedUser, Role assignedRole, Status newStatus)
    {
        foreach (var pInstance in MockData.AllInstances)
        {
            foreach (var task in pInstance.AllTasks)
            {
                if (task.Id == id)
                {
                    Console.WriteLine(id);
                    task.AssignedUser = assignedUser;
                    task.AssignedRole = assignedRole;
                    task.TaskStatus = newStatus;
                }
            }
        }
    }
}