using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ContractTypesViews
{
    public class DeleteModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DeleteModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public ContractType ContractType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.ContractType == null)
            {
                return NotFound();
            }

            var contracttype = await _context.ContractType.FirstOrDefaultAsync(m => m.ContractName == id);

            if (contracttype == null)
            {
                return NotFound();
            }
            else 
            {
                ContractType = contracttype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.ContractType == null)
            {
                return NotFound();
            }
            var contracttype = await _context.ContractType.FindAsync(id);

            if (contracttype != null)
            {
                ContractType = contracttype;
                _context.ContractType.Remove(ContractType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
