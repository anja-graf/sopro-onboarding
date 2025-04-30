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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly Admin _admin;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
            _admin = new Admin(context);
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public string RolesInput { get; set; }

        [BindProperty]
        public string DepartmentsInput { get; set; }

        public IList<Role> Roles { get; set; }
        public IList<Department> Departments { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //load the user with the LoadUser from db
            User = User.LoadUser(_context, id);
            if (User == null)
            {
                return NotFound();
            }
            //load the Roles and Departments TODO: new static function in Role and Department
            Roles = Role.GetAllRoles(_context);
            Departments = Department.GetAllDepartments(_context);

            RolesInput = string.Join(",", User.Roles.Select(r => r.RoleName));
            DepartmentsInput = string.Join(",", User.Departments.Select(d => d.DepartmentName));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(User.Password))
            {
                ModelState.Remove("User.Password");
            }
            if (!ModelState.IsValid)
            {
                //reload department and role list
                Roles = Role.GetAllRoles(_context);
                Departments = Department.GetAllDepartments(_context);
                return Page();
            }
            //Load user to update from db
            var userToUpdate = User.LoadUser(_context, id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            List<Role> updatedRoles = new List<Role>();
            List<Department> updatedDepartments = new List<Department>();

            if (!string.IsNullOrEmpty(RolesInput))
            {
                //split the input string and save in list
                var roleNames = RolesInput.Split(',').Select(r => r.Trim());
                updatedRoles = await _context.Role.Where(r => roleNames.Contains(r.RoleName)).ToListAsync();
            }

            if (!string.IsNullOrEmpty(DepartmentsInput))
            {
                //split the input string and save in list
                var departmentNames = DepartmentsInput.Split(',').Select(d => d.Trim());
                updatedDepartments = await _context.Department.Where(d => departmentNames.Contains(d.DepartmentName)).ToListAsync();
            }
            //update the User
            bool updateSuccess = _admin.UpdateUser(
                userToUpdate.Email,
                User.Name,
                User.IsBlocked,
                updatedRoles,
                updatedDepartments
            );

            if (updateSuccess)
            {
                return RedirectToPage("./Index_new");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to update user.");
                Roles = Role.GetAllRoles(_context);
                Departments = Department.GetAllDepartments(_context);
                return Page();
            }
        }
    }
}