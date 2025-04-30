using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Replay.Data;

namespace Replay.Model;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string RoleName { get; set; }
    
    [ValidateNever]
    public List<User> Users { get; set; } = new List<User>();
    [ValidateNever]
    public ICollection<ProcessBlueprint> ProcessBlueprints { get; set; } = new List<ProcessBlueprint>();
    [ValidateNever]
    public ICollection<TaskBlueprint> Tasks { get; set; } = new List<TaskBlueprint>();

    public void Save(ApplicationDbContext _context)
    {
        _context.Add(this);
        _context.SaveChanges();
    }

    public void Update(ApplicationDbContext _context, string newRoleName)
    {
        this.RoleName = newRoleName;
        _context.Update(this);
        _context.SaveChanges();
    }
    
    public static List<Role> GetAllRoles(ApplicationDbContext _context)
    {
        return _context.Role.ToList();
    }

    public static Role GetById(ApplicationDbContext _context, int id)
    {
        return _context.Role.Find(id);
    }

    public static List<Role> GetByIds(ApplicationDbContext _context, List<int> ids)
    {
        return _context.Role.Where(role => ids.Contains(role.Id)).ToList();
    }

    // Important to get 'Bezugsperson' & 'Vorgangsverantwortlicher'
    public static Role GetByName(ApplicationDbContext _context, string name)
    {
        return _context.Role.Where(r => r.RoleName == name).FirstOrDefault();
    }
}
