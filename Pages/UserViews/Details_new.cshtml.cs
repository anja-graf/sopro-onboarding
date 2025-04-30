using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay
{
    public class DetailsModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DetailsModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public User User { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            //calls the static LoadUser method to get User data from db
            User = User.LoadUser(_context, id);
            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}