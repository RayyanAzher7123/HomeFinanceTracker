using HomeFinance.web.Models;
using HomeFinance.web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;

namespace HomeFinance.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = _context.AppUsers.Any(u => u.Username == model.Username);
                if (userExists)
                {
                    ModelState.AddModelError("", "Username already taken");
                    return View(model);
                }

                var user = new AppUser
                {
                    Name = model.Name,
                    Username = model.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                // Redirect to Login page after successful registration
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUsers.FirstOrDefault(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // Login successful - set session or cookie as needed
                    // Example: TempData for now
                    TempData["Message"] = $"Welcome back, {user.Name}!";

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }

    }

}
