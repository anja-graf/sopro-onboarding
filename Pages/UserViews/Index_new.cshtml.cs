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
    public class IndexModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public IndexModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<User> User { get; set; } = default!;
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterOption { get; set; }

        public async Task OnGetAsync()
        {
            var users = from u in _context.User
                        select u;

            if (!string.IsNullOrEmpty(SearchString))
            {
                users = users.Where(u => u.Name.Contains(SearchString) || u.Email.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(FilterOption))
            {
                if (FilterOption == "blocked")
                {
                    users = users.Where(u => u.IsBlocked);
                }
                else if (FilterOption == "notblocked")
                {
                    users = users.Where(u => !u.IsBlocked);
                }
            }

            User = await users.ToListAsync();
        }
    }
}
