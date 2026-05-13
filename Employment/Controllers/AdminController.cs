using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.Models;
using Employment.ViewModels;
using Employment.Services;
using Microsoft.AspNetCore.Authorization;

namespace Employment.Controllers
{
    // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Jobs.ToListAsync();
            return View(jobs);
        }

        // GET: /Admin/CreateJob
        public IActionResult CreateJob()
        {
            return View(new AdminJobViewModel());
        }

        // POST: /Admin/CreateJob
        [HttpPost]
        public async Task<IActionResult> CreateJob(AdminJobViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var job = new Job
            {
                Title = vm.Title,
                Description = vm.Description,
                Department = vm.Department,
                Location = vm.Location,
                SalaryMin = vm.SalaryMin,
                SalaryMax = vm.SalaryMax,
                MinExperience = vm.MinExperience,
                RequiredEducation = vm.RequiredEducation,
                Status = "Active",
                CreatedBy = 1
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            // Save skills
            if (vm.Skills != null && vm.Skills.Any())
            {
                foreach (var skill in vm.Skills.Where(s => !string.IsNullOrEmpty(s.SkillName)))
                {
                    // Inside the foreach loop in CreateJob POST action
                    var jobSkill = new JobSkill
                    {
                        JobId = job.JobId,
                        SkillName = skill.SkillName,
                        MinYearOfExperience = skill.MinYearsOfExperience,
                        // Example mapping if ImportantLevel is an int in the database:
                        ImportantLevel = skill.ImportantLevel == "Required" ? 1 :
                                         skill.ImportantLevel == "Preferred" ? 2 : 3,
                        IsRequired = skill.IsRequired
                    };
                    _context.JobSkills.Add(jobSkill);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // GET: /Admin/EditJob/5
        public async Task<IActionResult> EditJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound();

            var vm = new AdminJobViewModel
            {
                JobId = job.JobId,
                Title = job.Title,
                Description = job.Description,
                Department = job.Department,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                MinExperience = job.MinExperience,
                RequiredEducation = job.RequiredEducation,
                Status = job.Status
            };

            return View(vm);
        }

        // POST: /Admin/EditJob/5
        [HttpPost]
        public async Task<IActionResult> EditJob(int id, AdminJobViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound();

            job.Title = vm.Title;
            job.Description = vm.Description;
            job.Department = vm.Department;
            job.Location = vm.Location;
            job.SalaryMin = vm.SalaryMin;
            job.SalaryMax = vm.SalaryMax;
            job.MinExperience = vm.MinExperience;
            job.RequiredEducation = vm.RequiredEducation;
            job.Status = vm.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Admin/Settings
        // GET: /Admin/Settings
        // GET: /Admin/Settings
        public async Task<IActionResult> Settings()
        {
            var settings = await _context.SystemSettings.ToListAsync();

            var vm = new SettingsViewModel
            {
                Settings = settings.Select(s => new SettingItemViewModel
                {
                    SettingKey = s.SettingKey,
                    SettingValue = s.SettingValue ?? "",
                    Description = s.Description ?? "",
                    IsWeight = s.SettingKey.ToLower().Contains("weight") ||
                                   s.SettingKey.ToLower().Contains("score")
                }).ToList()
            };

            return View(vm);
        }

        // POST: /Admin/Settings
        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel vm)
        {
            // Validate weights sum to 100
            var weightSettings = vm.Settings
                .Where(s => s.IsWeight)
                .ToList();

            if (weightSettings.Any())
            {
                var total = weightSettings
                    .Sum(s => double.TryParse(s.SettingValue, out var v) ? v : 0);

                if (Math.Abs(total - 100) > 0.01)
                {
                    var settings = await _context.SystemSettings.ToListAsync();
                    vm.Settings = settings.Select(s => new SettingItemViewModel
                    {
                        SettingKey = s.SettingKey,
                        SettingValue = vm.Settings.FirstOrDefault(x => x.SettingKey == s.SettingKey)?.SettingValue ?? s.SettingValue ?? "",
                        Description = s.Description ?? "",
                        IsWeight = s.SettingKey.ToLower().Contains("weight") ||
                                       s.SettingKey.ToLower().Contains("score")
                    }).ToList();

                    vm.ErrorMessage = $"Weight settings must sum to 100. Current total: {total}";
                    return View(vm);
                }
            }

            foreach (var setting in vm.Settings)
            {
                var existing = await _context.SystemSettings.FindAsync(setting.SettingKey);
                if (existing != null)
                {
                    existing.SettingValue = setting.SettingValue ?? "";
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Settings saved successfully!";
            return RedirectToAction("Settings");
        }

// POST: /Admin/GenerateDescription
[HttpPost]
public async Task<IActionResult> GenerateDescription(
    [FromBody] GenerateDescriptionRequest request,
    [FromServices] JobDescriptionService jobDescService)
{
    if (string.IsNullOrEmpty(request.Title))
        return BadRequest("Job title is required");

    var description = await jobDescService.GenerateJobDescriptionAsync(
        request.Title,
        request.Department ?? "General",
        request.Skills ?? new List<string>()
    );

    if (description == null)
        return StatusCode(500, "AI generation failed");

    return Ok(new { description });
}

public class GenerateDescriptionRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<string>? Skills { get; set; }
}

    }
}