using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Model;

namespace Replay.Pages.TaskViews;

public class TaskListTaskViewModel
{
    public int Id { get; set; }
    public String ProcessName { get; set; }
    public string TaskName { get; set; }
    public string ResponsibleUserName { get; set; }
    public string ReferenceUserName { get; set; }
    public DateOnly DueDate { get; set; }
    public Status Status { get; set; }
    public string Instructions { get; set; }
}