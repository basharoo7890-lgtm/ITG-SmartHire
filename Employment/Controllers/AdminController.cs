using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.Models;
using Employment.ViewModels;
using Employment.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Employment.Controllers
{
    // [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
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
                CreatedBy = GetCurrentUserId()
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
            // Validate weights sum to 100.
            // NOTE: don't trust the posted IsWeight hidden field - recompute
            // it from the setting key itself, same as the GET action does,
            // so a missing/tampered/unbound hidden field can't bypass validation.
            var weightSettings = vm.Settings
                .Where(s => s.SettingKey.ToLower().Contains("weight") ||
                            s.SettingKey.ToLower().Contains("score"))
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

        // POST: /Admin/DeleteJob/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound();

            // Delete related AI analyses first
            var appIds = await _context.Applications
                .Where(a => a.JobId == id)
                .Select(a => a.ApplicationId)
                .ToListAsync();

            var analyses = await _context.AIAnalyses
                .Where(a => appIds.Contains(a.ApplicationId))
                .ToListAsync();
            _context.AIAnalyses.RemoveRange(analyses);

            // Delete related applications
            var applications = await _context.Applications
                .Where(a => a.JobId == id)
                .ToListAsync();
            _context.Applications.RemoveRange(applications);

            // Delete related job skills
            var skills = await _context.JobSkills
                .Where(s => s.JobId == id)
                .ToListAsync();
            _context.JobSkills.RemoveRange(skills);

            // Now delete the job
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /Admin/Users
public async Task<IActionResult> Users()
{
    var users = await _context.Users
        .OrderBy(u => u.Role)
        .ThenBy(u => u.FullName)
        .ToListAsync();
    return View(users);
}

// POST: /Admin/ChangeRole
[HttpPost]
public async Task<IActionResult> ChangeRole(int userId, string role)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null) return NotFound();

    if (role == "Admin" || role == "HR" || role == "Applicant")
    {
        user.Role = role;
        await _context.SaveChangesAsync();
    }

    return RedirectToAction("Users");
}

// POST: /Admin/DeleteUser

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteUser(int userId)
{
    try
    {
        var user = await _context.Users
            .Include(u => u.Applications)
            .FirstOrDefaultAsync(u => u.UserId == userId);
            
        if (user == null)
        {
            TempData["Error"] = "User not found";
            return RedirectToAction(nameof(Users));
        }
        
        // Check if user has applications
        if (user.Applications != null && user.Applications.Any())
        {
            // Option 1: Delete applications first
            _context.Applications.RemoveRange(user.Applications);
            
            // Option 2: Or just warn and don't delete
            // TempData["Error"] = $"Cannot delete user with {user.Applications.Count} existing applications. Delete applications first.";
            // return RedirectToAction(nameof(Users));
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "User deleted successfully";
    }
    catch (DbUpdateException ex)
    {
        TempData["Error"] = "Cannot delete user because they have existing applications. Please delete their applications first.";
        _logger.LogError(ex, "Error deleting user {UserId}", userId);
    }
    
    return RedirectToAction(nameof(Users));
}

        // Helper: get current logged-in user ID from Claims
        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }
    }
}