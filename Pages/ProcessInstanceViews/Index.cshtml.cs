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
    public class IndexModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        

        public IndexModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ProcessInstance> ActiveProcessInstances { get;set; }
        public IList<ProcessInstance> ArchivedProcessInstances { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var allProcessInstances = await ProcessInstance.GetAllProcessInstancesAsync(_context);
            ActiveProcessInstances = allProcessInstances.Where(instance => !instance.IsArchived).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostArchive(int id)
        {
            var pInstance = await ProcessInstance.GetByIdAsync(_context, id);
            await pInstance.ArchiveProcessInstanceAsync(_context);
            await OnGetAsync();
            return Page();
        }
        



        /* public async Task OnGetAsync()
        {
            if (_context.ProcessInstance != null)
            {
                ProcessInstance = await _context.ProcessInstance.ToListAsync();
            }
        } */
    }
}
