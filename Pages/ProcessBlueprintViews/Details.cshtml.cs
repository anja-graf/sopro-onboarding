using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Model;

namespace Replay.Pages.ProcessBlueprintViews
{
    public class DetailsModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public DetailsModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        
        public List<Role> AvailableRoles;
        public List<ContractType> AvailableContractTypes;
        public List<Department> AvailableDepartments;
        public List<DueDateType> AvailableDueDateTypes;
        public List<TaskBlueprint> AllTasks { get; set; }
        
        [BindProperty]
        public ProcessBlueprint ToDisplay { get; set; }
        public TaskBlueprintTemp NewTaskBlueprint { get; set; }
        public string ToUpdateTaskBlueprintName { get; set; }
        public string NewProcessBlueprintRoleIds { get; set; }

        public bool IsUpdating { get; set; }
        public async Task<IActionResult> OnGetAsync(string toDetail)
        {

            // Get the ProcessBlueprint including the related Roles and Tasks
            ToDisplay = await ProcessBlueprint.FindByNameAsync(_context, toDetail);
            ReloadLists();
            NewProcessBlueprintRoleIds = string.Join(",", ToDisplay.PermittedRoles.Select(r => r.Id));
            NewTaskBlueprint = GetStaringTemplate();
            return Page();
        }
        public void ReloadLists()
        {
            AvailableRoles = Role.GetAllRoles(_context);
            AvailableContractTypes = ContractType.GetAllContractTypes(_context);
            AvailableDepartments = Department.GetAllDepartments(_context);
            AvailableDueDateTypes = DueDateTypeHelper.AllDueDayTypes();
            AllTasks = ToDisplay.Tasks;
        }

        public TaskBlueprintTemp GetStaringTemplate()
        {
            TaskBlueprintTemp ret = new TaskBlueprintTemp()
            {
                TaskName = String.Empty,
                DueDateType = DueDateType.ASAP.ToString(),
                PermittedRoleId = Role.GetAllRoles(_context).First().Id,
                PermittedDepartmentIds = string.Join(",", AvailableDepartments.Select(d => d.Id)),
                PermittedContractTypeIds = string.Join(",", AvailableContractTypes.Select(c => c.Id))
            };
            return ret;
        }
    }
}
