using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Exceptions;
using Replay.Model;

namespace Replay.Pages.ProcessInstanceViews
{
    public class EditModel : PageModel
    {
        private readonly Replay.Data.ApplicationDbContext _context;

        public EditModel(Replay.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProcessInstance EditProcessInstance { get; set; }
        [Required(ErrorMessage = "E-Mail Feld darf nicht leer sein!")]
        [BindProperty]
        public string EditResponsibleUserMail { get; set; }
        [BindProperty]
        public string EditDueDate { get; set; }

        [BindProperty] 
        public int ProcessInstanceId { get; set; }

        public List<User> AllUsers { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            ModelState.Clear();
            EditProcessInstance = await ProcessInstance.GetByIdAsync(_context,id);
            AllUsers = Model.User.GetAll(_context);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            User? refUser = null;
            DateOnly dueDate = new DateOnly();
            EditProcessInstance = await ProcessInstance.GetByIdAsync(_context, ProcessInstanceId);
            try
            {
                refUser = Model.User.LoadUser(_context, EditResponsibleUserMail);
            }
            catch (EntityNotFoundException)
            {
                ModelState.AddModelError("EditReferenceUserMail", "User mit angegebener Email nicht gefunden. Bitte existierende Email angeben!");
            }
            try
            {
                 dueDate = DateOnly.Parse(EditDueDate);
            }
            catch (ArgumentNullException)
            {
                ModelState.AddModelError("EditDueDate", "Bitte ein gültiges Datum eingeben");
            }

            if (ModelState.ErrorCount > 0)
            {
                await OnGetAsync(EditProcessInstance.Id);
                return Page();
            }

            EditProcessInstance.UpdateProcessInstance(_context, refUser, dueDate);
            return RedirectToPage("Index");

        }

        public async Task<IActionResult> OnPostDeleteTaskInstance(int taskInstanceId, int processInstanceId)
        {
            TaskInstance toDel = TaskInstance.GetTaskInstanceById(_context ,taskInstanceId);
            toDel.Remove(_context);

            EditProcessInstance = await ProcessInstance.GetByIdAsync(_context, processInstanceId);
            return await OnGetAsync(EditProcessInstance.Id);
        }

        private bool ProcessInstanceExists(int id)
        {
          return (_context.ProcessInstance?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
