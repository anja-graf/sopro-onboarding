using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;
using System.Threading.Tasks;

namespace Replay.Model;

public class ProcessInstance
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Prozessinstanz benötigt einen Namen")]
    public string ProcessInstanceName { get; set; }
    public DateOnly DueDate { get; set; }
    
    public bool IsArchived { get; set; }
    
    public List<TaskInstance> AllTasks { get; set; }
    public ContractType Contract { get; set; }
    public Department Department { get; set; }
    public User ResponsibleUser { get; set; }
    public User ReferenceUser { get; set; }
    
    public ProcessInstance()
    {
    }

    public void UpdateProcessInstance(ApplicationDbContext _context ,User newResponsibleUser, DateOnly newDueDate)
    {
        ResponsibleUser = newResponsibleUser;
        DueDate = newDueDate;
        _context.Update(this);
        _context.SaveChanges();
    }

    public async Task ArchiveProcessInstanceAsync(ApplicationDbContext _context)
    {
        IsArchived = true;
        _context.Update(this);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAsync(ApplicationDbContext _context)
    {
        await _context.ProcessInstance.AddAsync(this);
        await _context.SaveChangesAsync();
    }

    public static async Task<List<ProcessInstance>> GetAllProcessInstancesAsync(ApplicationDbContext _context)
    {
        return await _context.ProcessInstance
            .Include(pi => pi.AllTasks)
            .Include(pi => pi.Contract)
            .Include(pi => pi.ResponsibleUser)
            .Include(pi => pi.ReferenceUser)
            .ToListAsync();
    }

    public static async Task<ProcessInstance> GetByIdAsync(ApplicationDbContext _context, int id)
    {
        var ret = await _context.ProcessInstance
            .Include(pi => pi.AllTasks)
            .ThenInclude(task => task.AssignedRole)
            .Include(pi => pi.Contract)
            .Include(pi => pi.Department)
            .Include(pi => pi.ResponsibleUser)
            .Include(pi => pi.ReferenceUser)
            .FirstOrDefaultAsync(pi => pi.Id == id);
        if (ret == null) throw new EntityNotFoundException("Processinstance:" + id + " wasn't found");
        return ret;
    }
}