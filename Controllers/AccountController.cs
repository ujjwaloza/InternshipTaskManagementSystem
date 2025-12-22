using Microsoft.AspNetCore.Mvc;
using InternshipTaskManagementSystem.Data;
using Microsoft.Identity.Client;

namespace InternshipTaskManagementSystem.Controllers
{
    public class AccountController : Controller

    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user =_context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FullName);
                if(user.Role=="Admin")
                
                    return RedirectToAction("Dashboard", "Admin");
                
                if(user.Role=="Student")
                
                    return RedirectToAction("Dashboard", "Student");
                
                if(user.Role=="Mentor")
                
                    return RedirectToAction("Dashboard", "Mentor");
                
            }
            ViewBag.Error="Invalid email or password.";
            return View();

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
