using Microsoft.AspNetCore.Mvc;
using Employment.Interfaces;
using Employment.Models;
using Employment.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobService _jobService;

        public HomeController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Index()
        {
            var recentJobs = await _jobService.GetAllJobsAsync();
            return View(recentJobs.Take(3).ToList());
        }

        public async Task<IActionResult> OpenPositions(string title, string location)
        {
            var allJobs = await _jobService.GetAllJobsAsync();
            
            // Apply filters if provided
            if (!string.IsNullOrEmpty(title))
            {
                allJobs = allJobs.Where(j => j.Title.Contains(title, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(location))
            {
                allJobs = allJobs.Where(j => j.Location.Contains(location, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Get applied status if user is logged in
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = GetCurrentUserId();
                if (userId > 0 && allJobs.Any())
                {
                    var jobIds = allJobs.Select(j => j.JobId).ToList();
                    var appliedStatus = await _jobService.GetUserApplicationsStatusAsync(userId, jobIds);
                    ViewBag.AppliedJobs = appliedStatus;
                }
            }
            
            return View(allJobs);
        }

        public async Task<IActionResult> JobDetails(int id)
        {
            var viewModel = await _jobService.GetJobDetailsAsync(id);
            if (viewModel == null) return NotFound();
            
            // Check if current user has applied
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = GetCurrentUserId();
                if (userId > 0)
                {
                    ViewBag.AlreadyApplied = await _jobService.HasUserAppliedToJobAsync(userId, id);
                }
                else
                {
                    ViewBag.AlreadyApplied = false;
                }
            }
            else
            {
                ViewBag.AlreadyApplied = false;
            }
            
            return View("JobDetails", viewModel);
        }

        // Redirect any calls to "Details" to "JobDetails"
        public IActionResult Details(int id)
        {
            return RedirectToAction("JobDetails", new { id = id });
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
        
        private int GetCurrentUserId()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
            {
                return 0;
            }
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            return 0;
        }
    }
}