using InternshipTaskManagementSystem.Data;
using InternshipTaskManagementSystem.Models;
using InternshipTaskManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InternshipTaskManagementSystem.Controllers
{
    public class AdminController : Controller
    {
       
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AdminController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        //public IActionResult Users()
        //{
        //    if (!IsAdmin()) return RedirectToAction("Login", "Account");

        //    return View(_context.Users.ToList());
        //}
        [HttpGet]
        [Route("Admin/Users")]
        public IActionResult Users(int page = 1)
        {
            int pageSize = 5;

            var totalUsers = _context.Users.Count();

            var users = _context.Users
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            return View(users);
        }
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(user);

            string generatedPassword = Guid.NewGuid().ToString().Substring(0, 8);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            user.Password = hashedPassword;
            user.CreatedAt = DateTime.UtcNow;
            user.IsFirstLogin = true; 

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _emailService.SendEmail(
                user.Email,
                "Your Internship Portal Login Details",
                $@"
        <h2>Welcome to Internship Portal</h2>

        <p>Your account has been created successfully.</p>

        <p><b>Email:</b> {user.Email}</p>
        <p><b>Password:</b> {generatedPassword}</p>

        <p style='color:red;'>
            <a href='https://localhost:5001/Account/ChangePassword?email={user.Email}'>Change Password</a>
        </p>

        <br/>
        <p>Regards,<br/>Admin Team</p>
        "
            );

            return RedirectToAction("Users");
        }

        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);
            if (user == null) return RedirectToAction("Users");

            return View(user);
        }

        //[HttpPost]
        //public IActionResult EditUser(User user)
        //{
        //    if (!IsAdmin()) return RedirectToAction("Login", "Account");

        //    if (!ModelState.IsValid)
        //        return View(user);

        //    _context.Entry(user).Property(u => u.CreatedAt).IsModified = false;

        //    _context.Users.Update(user);
        //    _context.SaveChanges();

        //    return RedirectToAction("Users");
        //}
        private string GenerateRandomPassword()
        {
            return "ITMS@" + new Random().Next(1000, 9999);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(user);

            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser == null)
                return NotFound();

            // Keep old created date
            existingUser.CreatedAt = existingUser.CreatedAt;

            // Update basic fields
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            // 🔥 IF PASSWORD IS CHANGED
            if (!string.IsNullOrEmpty(user.Password))
            {
                // Generate random password
                var newPassword = GenerateRandomPassword();

                // Hash password
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Force change on login
                existingUser.IsFirstLogin = true;

                // 🔥 SEND EMAIL
                await _emailService.SendEmail(
                    existingUser.Email,
                    "Your Password Has Been Reset",
                    $"Hello {existingUser.FullName},<br/><br/>" +
                    $"Your password has been reset by admin.<br/><br/>" +
                    $"<b>New Password:</b> {newPassword}<br/><br/>" +
                    $"Please login and change your password immediately.<br/><br/>" +
                    $"Thank you."
                );
            }

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);
            if (user == null) return RedirectToAction("Users");

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteUserConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);
            if (user == null) return RedirectToAction("Users");

          
            bool hasProjects = _context.Projects
                .Any(p => p.StudentId == id || p.MentorId == id);

            if (hasProjects)
            {
                TempData["Error"] = "Cannot delete user. This user is assigned to one or more projects.";
                return RedirectToAction("Users");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Users");
        }


        public IActionResult Projects()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var projects = _context.Projects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .ToList();

            return View(projects);
        }

        public IActionResult CreateProject()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
            ViewBag.Mentors = _context.Users.Where(u => u.Role == "Mentor").ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProject(Project project)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"MODEL ERROR → {entry.Key}: {error.ErrorMessage}");
                    }
                }

                ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
                ViewBag.Mentors = _context.Users.Where(u => u.Role == "Mentor").ToList();

                return View(project);
            }
            

            _context.Projects.Add(project);
            _context.SaveChanges();

            Console.WriteLine("PROJECT SAVED SUCCESSFULLY");

            return RedirectToAction("Projects");
        }


        public IActionResult EditProject(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var project = _context.Projects.Find(id);
            if (project == null) return RedirectToAction("Projects");

            ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
            ViewBag.Mentors = _context.Users.Where(u => u.Role == "Mentor").ToList();

            return View(project);
        }

        [HttpPost]
        public IActionResult EditProject(Project project)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Students = _context.Users.Where(u => u.Role == "Student").ToList();
                ViewBag.Mentors = _context.Users.Where(u => u.Role == "Mentor").ToList();
                return View(project);
            }

            _context.Projects.Update(project);
            _context.SaveChanges();

            return RedirectToAction("Projects");
        }

        public IActionResult DeleteProject(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var project = _context.Projects
                .Include(p => p.Student)
                .Include(p => p.Mentor)
                .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
                return RedirectToAction("Projects");

            return View(project);
        }

        [HttpPost, ActionName("DeleteProject")]
        public IActionResult DeleteProjectConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var project = _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
                return RedirectToAction("Projects");

            
            if (project.Tasks != null && project.Tasks.Any())
            {
                _context.Tasks.RemoveRange(project.Tasks);
            }

            _context.Projects.Remove(project);
            _context.SaveChanges();

            return RedirectToAction("Projects");
        }


        public IActionResult WeeklyReports()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var reports = _context.WeeklyReports
                .Include(r => r.Student)
                .ToList();

            return View(reports);
        }
        public IActionResult CreateTask()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.Projects = _context.Projects
                .Include(p => p.Student)
                .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTask(TaskModel task)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Projects = _context.Projects
                    .Include(p => p.Student)
                    .ToList();

                return View(task);
            }

           
            var project = _context.Projects
                .Include(p => p.Student)
                .FirstOrDefault(p => p.ProjectId == task.ProjectId);

            if (project == null)
            {
                ModelState.AddModelError("", "Invalid Project");
                return View(task);
            }

            
            task.StudentId = project.StudentId;
            task.Status = "ToDo";
            task.CreatedAt = DateTime.UtcNow;

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Projects");
        }
        //public IActionResult DeleteMultiple(string ids)
        //{
        //    var idList = ids.Split(',').Select(int.Parse).ToList();

        //    var users = _context.Users.Where(u => idList.Contains(u.UserId)).ToList();

        //    foreach (var user in users)
        //    {
        //        if (user.Role != "Admin")
        //        {
        //            _context.Users.Remove(user);
        //        }
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("Users");
        //}



        public IActionResult DeleteMultiple(string ids)
        {
            var idList = ids.Split(',').Select(int.Parse).ToList();

            var users = _context.Users
                .Where(u => idList.Contains(u.UserId))
                .ToList();

            foreach (var user in users)
            {
                if (user.Role != "Admin")
                {
                    // 🔥 DELETE RELATED PROJECTS FIRST
                    var projects = _context.Projects
                        .Where(p => p.StudentId == user.UserId || p.MentorId == user.UserId)
                        .ToList();

                    _context.Projects.RemoveRange(projects);

                    // 🔥 DELETE REPORTS
                    var reports = _context.WeeklyReports
                        .Where(r => r.StudentId == user.UserId)
                        .ToList();

                    _context.WeeklyReports.RemoveRange(reports);

                    // 🔥 DELETE TASKS (if linked)
                    var tasks = _context.Tasks
                        .Where(t => t.StudentId == user.UserId)
                        .ToList();

                    _context.Tasks.RemoveRange(tasks);

                    // 🔥 NOW DELETE USER
                    _context.Users.Remove(user);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Users");
        }
        //public IActionResult FixPasswords()
        //{
        //    var users = _context.Users.ToList();

        //    foreach (var user in users)
        //    {
        //        // Check if password is NOT hashed
        //        if (!string.IsNullOrEmpty(user.Password) && !user.Password.StartsWith("$2"))
        //        {
        //            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        //        }
        //    }

        //    _context.SaveChanges();

        //    return Content("All passwords converted to BCrypt hash successfully!");
        //}

    }
}
