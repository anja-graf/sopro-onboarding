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
    public class DetailsModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DetailsModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
