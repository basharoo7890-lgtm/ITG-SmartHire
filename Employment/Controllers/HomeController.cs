using Employment.Data;
using Employment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Jobs
                .Where(j => j.Status == "Open")
                .OrderByDescending(j => j.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View(jobs);
        }

        public async Task<IActionResult> OpenPositions(string title, string location)
        {
            var jobs = _context.Jobs.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                jobs = jobs.Where(j => j.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(location))
            {
                jobs = jobs.Where(j => j.Location.Contains(location));
            }

            var jobList = await jobs
                .Where(j => j.Status == "Open")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobList);
        }

        public async Task<IActionResult> JobDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
                return NotFound();

            // Check if user already applied
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            var alreadyApplied = userId > 0 && await _context.Applications
                .AnyAsync(a => a.JobId == id && a.UserId == userId);

            ViewBag.AlreadyApplied = alreadyApplied;

            return View(job);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}