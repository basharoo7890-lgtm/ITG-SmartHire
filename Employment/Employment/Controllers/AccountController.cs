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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
      
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "الرجاء إدخال الإيميل وكلمة المرور");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
               
                return RedirectToAction("Index", "Home");
            }

           
            ModelState.AddModelError("", "خطأ: الإيميل أو كلمة المرور غير صحيحة");
            return View();
        }

    
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}