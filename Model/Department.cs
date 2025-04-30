using System.ComponentModel.DataAnnotations;
using Replay.Data;
using Replay.Exceptions;

namespace Replay.Model;


public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string DepartmentName { get; set; }

    public ICollection<TaskBlueprint> Tasks { get; set; } = new List<TaskBlueprint>();

    public List<User> Users { get; set; } = new List<User>();
    
    public static List<Department> GetAllDepartments(ApplicationDbContext _context)
    {
        return _context.Department.ToList();
    }

    public static Department GetById(ApplicationDbContext _context, int id)
    {
        var ret = _context.Department.Find(id);
        if (ret == null) throw new EntityNotFoundException("Department: " + id + " wasn't found");
        return ret;
    }

    public static List<Department> GetByIds(ApplicationDbContext _context, List<int> ids)
    {
        return _context.Department.Where(dep => ids.Contains(dep.Id)).ToList();
    }
}
