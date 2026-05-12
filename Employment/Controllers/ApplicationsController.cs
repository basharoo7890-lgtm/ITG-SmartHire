using Microsoft.AspNetCore.Mvc;
using Employment.Models;
using Employment.Data;
using Employment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employment.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationService _applicationService;

        public ApplicationsController(ApplicationDbContext context, IApplicationService applicationService)
        {
            _context = context;
            _applicationService = applicationService;
        }

        [HttpGet]
        public IActionResult Apply(int jobId, string title, string department, string location, string type)
        {
            ViewBag.JobId = jobId;
            ViewBag.JobTitle = title;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId, decimal expectedSalary, int experience, string phone, string education, IFormFile cvFile)
        {
            if (cvFile == null) return View();

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await cvFile.CopyToAsync(stream);
            }

            var app = new Application
            {
                JobId = jobId,
                UserId = 1,
                Phone = phone,
                ExpectedSalary = expectedSalary,
                YearsOfExperience = experience,
                EducationLevel = education,
                CVFileName = uniqueFileName,
                SubmittedAt = DateTime.Now,
                Status = "Pending"
            };

            _context.Applications.Add(app);
            await _context.SaveChangesAsync();
            await _applicationService.ProcessAutoFilterAsync(app.ApplicationId);

            return RedirectToAction("MyApplications");
        }

        public async Task<IActionResult> MyApplications()
        {
            return View(await _context.Applications.Include(a => a.Job).ToListAsync());
        }
    }
}