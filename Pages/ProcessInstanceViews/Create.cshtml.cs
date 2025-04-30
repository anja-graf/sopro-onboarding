using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Replay.Model;


namespace Replay.Pages.ProcessInstanceViews;

public class CreateModel : PageModel
{
    public IActionResult OnGet(String Blueprintname)
    {
        return Page();
    }

    [BindProperty] public ProcessInstance DisplayProcessInstance { get; set; } = new ProcessInstance();

    public IActionResult OnPost(String action)
    {
        return Page();
    }

    public ProcessBlueprint GetBlueprintByName(string name)
    {
        return null;
    }
}