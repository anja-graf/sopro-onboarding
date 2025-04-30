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
    public class DeleteModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DeleteModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ProcessBlueprint ProcessBlueprint { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.ProcessBlueprint == null)
            {
                return NotFound();
            }

            var processblueprint = await _context.ProcessBlueprint.FirstOrDefaultAsync(m => m.ProcessBlueprintName == id);

            if (processblueprint == null)
            {
                return NotFound();
            }
            else 
            {
                ProcessBlueprint = processblueprint;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.ProcessBlueprint == null)
            {
                return NotFound();
            }
            var processblueprint = await _context.ProcessBlueprint.FindAsync(id);

            if (processblueprint != null)
            {
                ProcessBlueprint = processblueprint;
                _context.ProcessBlueprint.Remove(ProcessBlueprint);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
