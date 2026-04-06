using Microsoft.AspNetCore.Mvc;
using SmartHire.Data;
using SmartHire.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SmartHire.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // database
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
           
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
              
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role), 
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

              
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

            
                if (user.Role == "Admin") return RedirectToAction("Index", "Admin");
                if (user.Role == "HR") return RedirectToAction("Index", "Dashboard");

                return RedirectToAction("Index", "Home"); 
            }

            
            ViewBag.Error = "الايميل أو كلمة المرور غير صحيحة";
            return View();
        }

        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                
                var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (exists)
                {
                    ViewBag.Error = "هذا الايميل مستخدم من قبل!";
                    return View(user);
                }

                user.CreatedAt = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(user);
        }

      
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}