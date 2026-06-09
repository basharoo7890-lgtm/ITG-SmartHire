using Microsoft.AspNetCore.Mvc;
using Employment.Interfaces;
using Employment.Models;
using Employment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ApplicationDbContext _context;

        public JobsController(IJobService jobService, ApplicationDbContext context)
        {
            _jobService = jobService;
            _context = context;
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int id)
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
            }
            
            // Explicitly return the correct view
            return View("~/Views/Home/JobDetails.cshtml", viewModel);
        }

        // Other methods...

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