using InternshipTaskManagementSystem.Data;
using InternshipTaskManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternshipTaskManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

       
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
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

        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View(_context.Users.ToList());
        }

        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(user);

            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(id);
            if (user == null) return RedirectToAction("Users");

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(user);

            _context.Entry(user).Property(u => u.CreatedAt).IsModified = false;

            _context.Users.Update(user);
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
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var project = _context.Projects.Find(id);
            if (project == null) return RedirectToAction("Projects");

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
    }
}
