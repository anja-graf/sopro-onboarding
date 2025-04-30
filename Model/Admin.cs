using Microsoft.EntityFrameworkCore;
namespace Replay.Model;
using Replay.Data;

public class Admin : User
{
    private readonly ApplicationDbContext _context;

    public Admin(Replay.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    
    //checks if user exist with this email, creates a User with the input data
    //Robert
    public bool CreateUser(string name, string email, string password, List<Role> roles, List<Department> departments)
    {
        if (_context.User.Any(u => u.Email == email))
        {
            return false;
        }
        User newUser = new User(name, email, password);
        newUser.Roles = roles;
        newUser.Departments = departments;
        _context.User.Add(newUser);
        _context.SaveChanges();
        return true;
    }

    //admin archives a processInstance directly
    //Robert
    public bool ArchiveProcess(ProcessInstance processInstance)
    {
        if (processInstance != null)
        {
            processInstance.IsArchived = true;
            return true;
        }
        
        return false;
    }
    //can update user-name, user-blocked-state and add new departments and roles
    //Robert
    public bool UpdateUser(string email, string name, bool isBlocked, List<Role> roles, List<Department> departments)
    {
        //load user from Database
        var user = _context.User
            .Include(u => u.Roles)
            .Include(u => u.Departments)
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return false;
        }
        //set new Name and blocked state
        user.Name = name;
        user.IsBlocked = isBlocked;

        // Clear existing roles and departments
        user.Roles.Clear();
        user.Departments.Clear();

        // Add new roles and departments
        user.Roles.AddRange(roles);
        user.Departments.AddRange(departments);
        _context.SaveChanges();
        return true;
    }
}