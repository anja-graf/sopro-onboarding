using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;

namespace Replay.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProcessBlueprint
{
    [Key]
    [StringLength(25)]
    public string ProcessBlueprintName { get; set; }
    public List<TaskBlueprint> Tasks { get; set; } 
    public ICollection<Role> PermittedRoles {get; set;}
   
    
    public ProcessBlueprint(string processBlueprintName, List<TaskBlueprint> tasks, List<Role> permittedRoles)
    {
        ProcessBlueprintName = processBlueprintName;
        Tasks = tasks;
        PermittedRoles = permittedRoles;
    }

    public ProcessBlueprint()
    {

    }

    public async Task CreateAsync(ApplicationDbContext _context)
    {
        await _context.ProcessBlueprint.AddAsync(this);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ApplicationDbContext _context)
    {
        _context.ProcessBlueprint.Update(this);
        await _context.SaveChangesAsync();
    }
    
    public static async Task<ProcessBlueprint> FindByNameAsync(ApplicationDbContext _context, string name)
    {
        var ret = await _context.ProcessBlueprint
            .Include(pb => pb.PermittedRoles)
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedRole)
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedContractTypes)
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedDepartments)
            .FirstOrDefaultAsync(pb => pb.ProcessBlueprintName == name);
        if (ret == null) throw new EntityNotFoundException("Processblueprint: " + name + " was not found in db");
        return ret;
    }

    public static async Task<bool> ExistsAsync(ApplicationDbContext _context, string name)
    {
        var ret = await _context.ProcessBlueprint.FindAsync(name);
        return ret != null;
    }

    public static async Task<List<ProcessBlueprint>> GetAllProcessBlueprintsAsync(ApplicationDbContext _context)
    {
        return await _context.ProcessBlueprint
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedRole)
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedDepartments)
            .Include(pb => pb.Tasks)
            .ThenInclude(t => t.PermittedContractTypes)
            .Include(pb => pb.PermittedRoles)
            .ToListAsync();
    }
    
    public bool UpdateProcessBlueprint(List<TaskBlueprint> newTasks, List<Role> newRoles, string newProcessBlueprintName)
    {
        if (newTasks != null && newProcessBlueprintName != null && newRoles != null) // either update everything or nothing - 
        {
            ProcessBlueprintName = newProcessBlueprintName;
            Tasks = newTasks;
            PermittedRoles = newRoles;
            return true;
        }
        return false;
    }

    public async Task RemoveAsync(ApplicationDbContext _context)
    {
        _context.Remove(this);
        await _context.SaveChangesAsync();
    }
}