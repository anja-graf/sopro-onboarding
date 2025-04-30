using System.ComponentModel.DataAnnotations;
using Replay.Data;
using Replay.Exceptions;

namespace Replay.Model;

public class ContractType
{
    public ContractType()
    {
        
    }
    [Key]
    public int Id { get; set; }

    [Required]
    public string ContractName { get; set; }

    public ICollection<TaskBlueprint> Tasks { get; set; } = new List<TaskBlueprint>();


    public void Save(ApplicationDbContext _context)
    {
        _context.ContractType.Add(this);
        _context.SaveChanges();
    }

    public void Update(ApplicationDbContext _context, string newContractName)
    {
        this.ContractName = newContractName;
        _context.Update(this);
        _context.SaveChanges();
    }
    
    public static List<ContractType> GetAllContractTypes(ApplicationDbContext _context)
    {
        return _context.ContractType.ToList();
    }

    public static ContractType FindById(ApplicationDbContext _context ,int id)
    {
        var ret =  _context.ContractType.Find(id);
        if (ret == null) throw new EntityNotFoundException("ContractType: " + id + " wasn't found");
        return ret;
    }

    public static List<ContractType> FindByIds(ApplicationDbContext _context, List<int> ids)
    {
        return _context.ContractType.Where(con => ids.Contains(con.Id)).ToList();
    }
}