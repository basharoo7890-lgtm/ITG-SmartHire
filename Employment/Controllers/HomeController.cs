using Employment.Models;
using Employment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Employment.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class HomeController : Controller
    {
      private readonly IJobService _jobService;
private readonly ApplicationDbContext _context;

public HomeController(IJobService jobService, ApplicationDbContext context)
{
    _jobService = jobService;
    _context = context;
}

        public async Task<IActionResult> Index()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return View(jobs);
        }


        public async Task<IActionResult> JobDetails(int? id)
        {
            if (id == null)
                return View();

            var viewModel = await _jobService.GetJobDetailsAsync(id.Value);
            if (viewModel == null)
                return NotFound();

            // Check if user already applied
            var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
            var alreadyApplied = userId > 0 && await _context.Applications
                .AnyAsync(a => a.JobId == id.Value && a.UserId == userId);

            ViewBag.AlreadyApplied = alreadyApplied;

            return View(viewModel);
        }

        public async Task<IActionResult> OpenPositions()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return View(jobs);
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
