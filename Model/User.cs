using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Replay.Data;
using Replay.Exceptions;

namespace Replay.Model
{
    public class User
    {
        [Required(ErrorMessage = "Der Name ist erforderlich.")]
        public string Name { get; set; }
        [Key]
        [Required(ErrorMessage = "Email ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültiges Email-Format.")]
        public string Email { get; set; }
        //password is not mapped, so it won't be saved in the databse
        //[Required(ErrorMessage = "Das Passwort ist erforderlich.")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string? Password { get; set; }
        //instead the hashed password will be stored in db
        public string? PasswordHash { get; set; }
        public bool IsBlocked { get; set; }
        [ValidateNever]
        public List<Role> Roles { get; set; }
        [ValidateNever]
        public List<Department> Departments { get; set; }
        [ValidateNever]
        public List<TaskInstance> Tasks { get; set; }

        public User()
        {
            Roles = new List<Role>();
            Departments = new List<Department>();
            Tasks = new List<TaskInstance>();
        }


        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            SetPassword(password);
            IsBlocked = false;
            Roles = new List<Role>();
            Departments = new List<Department>();
            Tasks = new List<TaskInstance>();
        }

        //checks if password is empty, hashes the password, and stores hashed password
        //Robert
        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }
            if (ValidatePassword(password))
            {
                var hasher = new PasswordHasher<User>();
                PasswordHash = hasher.HashPassword(this, password);
            }
            else
            {
                throw new ArgumentException("Das Passwort erfüllt nicht die Anforderungen.");
            }
        }

        //test if password has: Uppercase, lowercase, digit, specialcharacter
        //Robert
        private bool ValidatePassword(string password)
        {
            var hasUpperCaseLetter = new Regex(@"[A-Z]+");
            var hasLowerCaseLetter = new Regex(@"[a-z]+");
            var hasDigit = new Regex(@"\d+");
            var hasSpecialChar = new Regex(@"[\W_]+");

            return password.Length >= 8 &&
                   hasUpperCaseLetter.IsMatch(password) &&
                   hasLowerCaseLetter.IsMatch(password) &&
                   hasDigit.IsMatch(password) &&
                   hasSpecialChar.IsMatch(password);
        }

        //load a User from db(for detail view)
        // Carlo: added exception: _context should never be null
        public static User LoadUser(ApplicationDbContext context, string email)
        {
            
            var ret = context.User
                .Include(u => u.Roles)
                .Include(u => u.Departments)
                .Include(u => u.Tasks)
                .FirstOrDefault(u => u.Email == email);
            if (ret == null) throw new EntityNotFoundException("User: " + email + " wasn't found");
            return ret;
        }

        //adds a task to the List of the User
        //Robert
        public bool PickTask(TaskInstance taskInstance)
        {
            if (taskInstance != null && !Tasks.Contains(taskInstance))
            {
                Tasks.Add(taskInstance);
                return true;
            }
            return false;
        }

        //checks if list contains a task, if yes then it removes it
        //Robert
        public bool RemoveTask(TaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                return false;
            }
            return Tasks.Remove(taskInstance);
        }

        //can be used for login. Checks if input password equals hashed password
        //Robert
        public bool VerifyPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(this, PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        public static List<User> GetAll(ApplicationDbContext _context)
        {
            return _context.User.ToList();
        }
    }
}