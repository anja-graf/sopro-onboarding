using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ProcessInstanceViews
{
    public class DeleteModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DeleteModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ProcessInstance ProcessInstance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ProcessInstance == null)
            {
                return NotFound();
            }

            var processinstance = await _context.ProcessInstance.FirstOrDefaultAsync(m => m.Id == id);

            if (processinstance == null)
            {
                return NotFound();
            }
            else 
            {
                ProcessInstance = processinstance;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.ProcessInstance == null)
            {
                return NotFound();
            }
            var processinstance = await _context.ProcessInstance.FindAsync(id);

            if (processinstance != null)
            {
                ProcessInstance = processinstance;
                _context.ProcessInstance.Remove(ProcessInstance);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
