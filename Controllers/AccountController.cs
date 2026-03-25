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
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            bool isValid = false;

            try
            {
                // ✅ If password is hashed
                if (user.Password.StartsWith("$2"))
                {
                    isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                }
                else
                {
                    // ⚠️ Old plain-text password
                    isValid = (password == user.Password);

                    // 🔥 Auto convert to BCrypt
                    if (isValid)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                ViewBag.Error = "Password error. Please reset password.";
                return View();
            }

            if (isValid)
            {
                // ✅ Session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FullName);

                // ✅ First login redirect
                if (user.IsFirstLogin)
                {
                    return RedirectToAction("ChangePassword", "Account");
                }

                // ✅ Role-based redirect
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                if (user.Role == "Student")
                    return RedirectToAction("Dashboard", "Student");

                if (user.Role == "Mentor")
                    return RedirectToAction("Dashboard", "Mentor");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            return View(new ChangePasswordDto
            {
                UserId = int.Parse(userId)
            });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View(model);
            }

            var user = await _context.Users.FindAsync(model.UserId);

            if (user == null)
                return RedirectToAction("Login");

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            user.IsFirstLogin = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
