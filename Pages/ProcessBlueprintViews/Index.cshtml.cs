using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ProcessBlueprintViews
{
    public class IndexModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;
        public List<ProcessBlueprint> AllBlueprints { get; set; }

        public IndexModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the list of created objects
            AllBlueprints = await ProcessBlueprint.GetAllProcessBlueprintsAsync(_context);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string toDel)
        {
            ProcessBlueprint remove = await ProcessBlueprint.FindByNameAsync(_context, toDel);
            await remove.RemoveAsync(_context);
            return RedirectToPage();
        }
    }
}