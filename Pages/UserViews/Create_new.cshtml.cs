using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Replay.Data;
using Replay.Model;

namespace Replay
{
    public class CreateModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;
        private readonly Admin _admin;

        public CreateModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
            _admin = new Admin(_context);
        }

        [BindProperty]
        public User User { get; set; } = new User();

        public IList<Role> Roles { get; set; } = new List<Role>();

        public IList<Department> Departments { get; set; } = new List<Department>();
        

        [BindProperty]
        public string RolesInput { get; set; } = string.Empty;

        [BindProperty]
        public string DepartmentsInput { get; set; } = string.Empty;

        //get request on page loading
        public async Task OnGetAsync()
        {
            Roles = Role.GetAllRoles(_context);
            Departments = Department.GetAllDepartments(_context);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) //Reloading the Role and department list
            {
                Roles = Role.GetAllRoles(_context);
                Departments = Department.GetAllDepartments(_context);
                return Page();
            }
            //hash the password
            User.SetPassword(User.Password);

            List<Role> selectedRoles = new List<Role>();
            List<Department> selectedDepartments = new List<Department>();
            
            //save the selected roles and departments in a List
            if (!string.IsNullOrEmpty(RolesInput))
            {
                var roleNames = RolesInput.Split(',').Select(role => role.Trim());
                selectedRoles = await _context.Role.Where(r => roleNames.Contains(r.RoleName)).ToListAsync();
            }

            if (!string.IsNullOrEmpty(DepartmentsInput))
            {
                var departmentNames = DepartmentsInput.Split(',').Select(department => department.Trim());
                selectedDepartments = await _context.Department.Where(d => departmentNames.Contains(d.DepartmentName)).ToListAsync();
            }
            //create a new User with the input data
            bool userCreated = _admin.CreateUser(User.Name, User.Email, User.PasswordHash, selectedRoles, selectedDepartments);

            //if the save was successfull, wait until create saves to database
            if (userCreated)
            {
                return RedirectToPage("./Index_new");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Benutzer konnte nicht erstellt werden");
                Roles = Role.GetAllRoles(_context);
                Departments = Department.GetAllDepartments(_context);
                return Page();
            }
        }
    }
}




