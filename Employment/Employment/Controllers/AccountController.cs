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
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

     
    }
}