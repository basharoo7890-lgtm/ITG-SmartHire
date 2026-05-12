using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Employment.Data;
using Employment.Models;

namespace Employment.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user, string ConfirmPassword)
        {
            if (user.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(user);
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("", "Email already registered");
                return View(user);
            }

          
            user.PasswordHash = HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.Now;

         
            if (user.Role != "Applicant" && user.Role != "HR" && user.Role != "Admin")
                user.Role = "Applicant";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string PasswordHash)
        {
            var hashed = HashPassword(PasswordHash);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashed);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

           
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "Dashboard"),
                "HR" => RedirectToAction("Index", "Dashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}