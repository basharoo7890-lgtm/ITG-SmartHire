using Microsoft.AspNetCore.Mvc;
using SmartHire.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartHire.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var latestJobs = await _context.Jobs
                .Where(j => j.Status == "Active")
                .OrderByDescending(j => j.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(latestJobs);
        }
    }
}
