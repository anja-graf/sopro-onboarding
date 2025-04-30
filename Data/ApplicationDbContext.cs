using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Replay.Model;

namespace Replay.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<ContractType> ContractType { get; set; }
        public DbSet<ProcessBlueprint> ProcessBlueprint { get; set; }
        public DbSet<ProcessInstance> ProcessInstance { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<TaskBlueprint> TaskBlueprint { get; set; }
        
       protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ProcessBlueprint zu TaskBlueprint (1:n)
    modelBuilder.Entity<ProcessBlueprint>()
        .HasMany(p => p.Tasks)
        .WithOne(t => t.ProcessBlueprint)
        .HasForeignKey(t => t.ProcessBlueprintName)
        .OnDelete(DeleteBehavior.Cascade);

    // Many-to-many relationship between ProcessBlueprint and Role
    modelBuilder.Entity<ProcessBlueprint>()
        .HasMany(p => p.PermittedRoles)
        .WithMany(r => r.ProcessBlueprints)
        .UsingEntity<Dictionary<string, object>>(
            "ProcessBlueprintRoles",
            j => j.HasOne<Role>().WithMany().OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<ProcessBlueprint>().WithMany().OnDelete(DeleteBehavior.Cascade));

    // Many-to-many relationship between TaskBlueprint and Department
    modelBuilder.Entity<TaskBlueprint>()
        .HasMany(t => t.PermittedDepartments)
        .WithMany(d => d.Tasks)
        .UsingEntity<Dictionary<string, object>>(
            "TaskBlueprintDepartments",
            j => j.HasOne<Department>().WithMany().OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<TaskBlueprint>().WithMany().OnDelete(DeleteBehavior.Cascade));

    // Many-to-many relationship between TaskBlueprint and ContractType
    modelBuilder.Entity<TaskBlueprint>()
        .HasMany(t => t.PermittedContractTypes)
        .WithMany(c => c.Tasks)
        .UsingEntity<Dictionary<string, object>>(
            "TaskBlueprintContractTypes",
            j => j.HasOne<ContractType>().WithMany().OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<TaskBlueprint>().WithMany().OnDelete(DeleteBehavior.Cascade));

    // Many-to-one relationship between TaskBlueprint and Role
    modelBuilder.Entity<TaskBlueprint>()
        .HasOne(t => t.PermittedRole)
        .WithMany(r => r.Tasks)
        .HasForeignKey(t => t.PermittedRoleId)
        .OnDelete(DeleteBehavior.Cascade);

    // Many-to-many relationship between User and Role
    modelBuilder.Entity<User>()
        .HasMany(u => u.Roles)
        .WithMany(r => r.Users)
        .UsingEntity<Dictionary<string, object>>(
            "UserRoles",
            j => j.HasOne<Role>().WithMany().OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<User>().WithMany().OnDelete(DeleteBehavior.Cascade));

    // Many-to-many relationship between User and Department
    modelBuilder.Entity<User>()
        .HasMany(u => u.Departments)
        .WithMany(d => d.Users)
        .UsingEntity<Dictionary<string, object>>(
            "UserDepartments",
            j => j.HasOne<Department>().WithMany().OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<User>().WithMany().OnDelete(DeleteBehavior.Cascade));
}
        public void InitializeRoles()
        {
            var roles = new[]
            {
                new Role { RoleName = "Administrator" },
                new Role { RoleName = "IT" },
                new Role { RoleName = "Backoffice" },
                new Role { RoleName = "Geschäftsleitung" },
                new Role { RoleName = "Personal" }
            };

            if (!Role.Any())
            {
                Role.AddRange(roles);
                SaveChanges();
            }
        }
        //Initializes Departments in db, if table is empty
        public void InitializeDepartments()
        {
            var departments = new[]
            {
                new Department { DepartmentName = "Entwicklung" },
                new Department { DepartmentName = "Operations" },
                new Department { DepartmentName = "UI/UX" },
                new Department { DepartmentName = "Projektmanagement" },
                new Department { DepartmentName = "Backoffice" },
                new Department { DepartmentName = "People & Culture" },
                new Department { DepartmentName = "Sales" }
            };
            if (!Department.Any())
            {
                Department.AddRange(departments);
                SaveChanges();
            }
        }
       
    }
}