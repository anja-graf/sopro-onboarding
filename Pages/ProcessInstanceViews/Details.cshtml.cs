using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;
using Replay.Model;

namespace Replay.Pages.ProcessInstanceViews
{
    public class DetailsModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DetailsModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProcessInstance DetailProcessInstance { get; set; }
        [BindProperty]
        public string DetailReferenceUserMail { get; set; }
        [BindProperty]
        public string DetailDueDate { get; set; }
        public ProcessInstance ProcessInstance { get; set; } = default!;
        public List<User> AllUsers { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                DetailProcessInstance = await ProcessInstance.GetByIdAsync(_context,id);
            }
            catch (EntityNotFoundException)
            {
                // return to index view if not found
                return RedirectToPage("./Index");
            }
            AllUsers = Model.User.GetAll(_context);
            return Page();
        }
    }
    
}
