using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ContractTypesViews
{
    public class CreateModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public CreateModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ContractType ContractType { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.ContractType == null || ContractType == null)
            {
                return Page();
            }

            _context.ContractType.Add(ContractType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
