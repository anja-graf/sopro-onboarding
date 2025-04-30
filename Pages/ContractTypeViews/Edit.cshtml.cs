using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ContractTypesViews
{
    public class EditModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public EditModel(Replay.Data.ApplicationDbContext context)
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

            var contracttype =  await _context.ContractType.FirstOrDefaultAsync(m => m.ContractName == id);
            if (contracttype == null)
            {
                return NotFound();
            }
            ContractType = contracttype;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ContractType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractTypeExists(ContractType.ContractName))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ContractTypeExists(string id)
        {
          return (_context.ContractType?.Any(e => e.ContractName == id)).GetValueOrDefault();
        }
    }
}
