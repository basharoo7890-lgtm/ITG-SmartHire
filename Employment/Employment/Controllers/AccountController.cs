using Microsoft.AspNetCore.Mvc;
using Employment.Data;
using Employment.Models;
using System.Linq;

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
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string PasswordHash)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == PasswordHash);
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "خطأ في البريد الإلكتروني أو كلمة المرور");
            return View();
        }
    }
}