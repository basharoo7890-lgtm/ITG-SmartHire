using Microsoft.AspNetCore.Mvc;
using Employment.Models;
using Employment.Data;
using Employment.Interfaces;
using Microsoft.EntityFrameworkCore;
using Employment.Services;
using System.Security.Claims;

namespace Employment.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationService _applicationService;
private readonly AIAnalysisService _aiService;
        public ApplicationsController(ApplicationDbContext context, IApplicationService applicationService, AIAnalysisService aiService)
{
    _context = context;
    _applicationService = applicationService;
    _aiService = aiService;
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
    // 1. Check if the Job actually exists in the DB first
    var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobId);
    if (!jobExists)
    {
        return BadRequest("The Job ID provided does not exist.");
    }

    // 2. Check if a file was uploaded
    if (cvFile == null || cvFile.Length == 0) 
    {
        ModelState.AddModelError("", "Please upload a CV file.");
        return View();
    }

    // 3. Define the uniqueFileName HERE so it's available for the rest of the method
    var uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
    
    if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

    // 4. Save the file to the folder
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await cvFile.CopyToAsync(stream);
    }

    // 5. Create the application object (uniqueFileName is now defined and safe to use)
    var app = new Application
    {
        JobId = jobId,
      UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1"),
        Phone = phone,
        ExpectedSalary = expectedSalary,
        YearsOfExperience = experience, 
        EducationLevel = education,
        CVFileName = uniqueFileName, // This matches the variable defined in Step 3
        SubmittedAt = DateTime.Now,
        Status = "Pending"
    };

    _context.Applications.Add(app);
    await _context.SaveChangesAsync();
    
    // Step 1: Auto filter
await _applicationService.ProcessAutoFilterAsync(app.ApplicationId);

// Step 2: Run AI pipeline in background
var appId = app.ApplicationId;
var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(2000);
        using var scope = scopeFactory.CreateScope();
        var aiService = scope.ServiceProvider.GetRequiredService<AIAnalysisService>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var updatedApp = await context.Applications.FindAsync(appId);
        if (updatedApp?.Status != "AutoRejected")
        {
            await aiService.AnalyzeApplicationAsync(appId);
            Console.WriteLine($"[Pipeline] ✅ AI analysis complete for {appId}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Pipeline] ❌ Error: {ex.Message}");
    }
});

return RedirectToAction("MyApplications");
  
}
     

        public async Task<IActionResult> MyApplications()
        {
            return View(await _context.Applications.Include(a => a.Job).ToListAsync());
        }
    }
}