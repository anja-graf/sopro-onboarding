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
    public class ArchivedModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public ArchivedModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IList<ProcessInstance> ArchivedProcessInstances { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var temp = await ProcessInstance.GetAllProcessInstancesAsync(_context);
            ArchivedProcessInstances = temp.Where(instance => instance.IsArchived == true).ToList();
            return Page();
        }
    }
}