using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Replay.Pages.ProcessBlueprintViews;

public class SelectBlueprint : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
}