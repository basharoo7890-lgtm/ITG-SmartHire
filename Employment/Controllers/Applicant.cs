using Employment.Data;
using Employment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicantController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Show Apply page
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Apply(int jobId)
        {
            if (jobId <= 0)
            {
                return RedirectToAction("OpenPositions", "Home");
            }

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);

            if (job == null)
            {
                return NotFound();
            }

            ViewBag.JobId = job.JobId;
            ViewBag.JobTitle = job.Title;
            ViewBag.Department = job.Department;
            ViewBag.Location = job.Location;
            ViewBag.JobStatus = job.Status;

            return View();
        }

        // Submit application
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(
            int jobId,
            string phone,
            decimal expectedSalary,
            int yearsOfExperience,
            string educationLevel,
            IFormFile cvFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);

            if (job == null)
            {
                ModelState.AddModelError("", "Invalid job selected.");
                return View();
            }

            ViewBag.JobId = job.JobId;
            ViewBag.JobTitle = job.Title;
            ViewBag.Department = job.Department;
            ViewBag.Location = job.Location;
            ViewBag.JobStatus = job.Status;

            if (cvFile == null || cvFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload your CV.");
                return View();
            }

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(cvFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Only PDF, DOC, and DOCX files are allowed.");
                return View();
            }

            if (cvFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "File size must be less than 5MB.");
                return View();
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "cvs"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await cvFile.CopyToAsync(stream);
            }

            var application = new Application
            {
                JobId = jobId,
                UserId = int.Parse(userId),
                Phone = phone,
                ExpectedSalary = expectedSalary,
                YearsOfExperience = yearsOfExperience,
                EducationLevel = educationLevel,
                CVFileName = uniqueFileName,
                CVText = "",
                Status = "Pending",
                SubmittedAt = DateTime.Now
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyApplications");
        }

        [Authorize]
        public async Task<IActionResult> MyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var applications = await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.UserId == int.Parse(userId))
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return View(applications);
        }
    }
}