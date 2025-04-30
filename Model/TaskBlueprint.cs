using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;
using Replay.Model;
using System.Threading.Tasks;

public class TaskBlueprint
{
    [Key]
    [Required]
    public string? TaskName { get; set; }

    public DueDateType DueDateType { get; set; }
    
    public int DaysRelativeToDueDate { get; set; }
    
    public string? Instructions { get; set; }

    // Foreign Key to Role
    public int PermittedRoleId { get; set; }
    [Required]
    public Role? PermittedRole { get; set; }

    // Many-to-many relationship with ContractType
    [ValidateNever]
    public ICollection<ContractType> PermittedContractTypes { get; set; } = new List<ContractType>();

    // Many-to-many relationship with Department
    [ValidateNever]
    public ICollection<Department> PermittedDepartments { get; set; } = new List<Department>();

    // Foreign Key to ProcessBlueprint
    public string? ProcessBlueprintName { get; set; }
    [ValidateNever]
    public ProcessBlueprint? ProcessBlueprint { get; set; }

    public async Task UpdateAsync(ApplicationDbContext _context)
    {
        _context.TaskBlueprint.Update(this);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemoveAsync(ApplicationDbContext _context)
    {
        _context.Remove(this);
        await _context.SaveChangesAsync();
    }

    public async Task SaveAsync(ApplicationDbContext _context)
    {
        await _context.AddAsync(this);
        await _context.SaveChangesAsync();
    }
    
    public static async Task<TaskBlueprint> FindByIdAsync(ApplicationDbContext _context, string name)
    {
        var taskBlueprint = await _context.TaskBlueprint
            .Include(tb => tb.PermittedContractTypes)
            .Include(tb => tb.PermittedDepartments)
            .Include(tb => tb.PermittedRole)
            .Include(tb => tb.ProcessBlueprint)
            .FirstOrDefaultAsync(tb => tb.TaskName == name);

        if (taskBlueprint == null)
            throw new EntityNotFoundException("Die Taskblueprint " + name + " existiert nicht!");
        
        return taskBlueprint;
    }
    
    public static async Task<bool> ExistsAsync(ApplicationDbContext _context, string name)
    {
        var ret = await _context.TaskBlueprint.FindAsync(name);
        return ret != null;
    }
}