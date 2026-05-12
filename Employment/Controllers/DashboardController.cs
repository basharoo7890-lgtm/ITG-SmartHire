using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.ViewModels;



namespace Employment.Controllers
{

    // [Authorize(Roles = "HR")]  // uncomment when Identity is set up by Omar
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard/Index?jobId=1
        // Lists all candidates for a job, sorted by MatchingScore descending
        public async Task<IActionResult> Index(int? jobId)
        {
            var jobs = await _context.Jobs.ToListAsync();

            // Default to first job if none selected
            var selectedJobId = jobId ?? jobs.FirstOrDefault()?.JobId ?? 0;

            var candidates = await _context.Applications
                .Where(a => a.JobId == selectedJobId)
                .Include(a => a.User)
                .Include(a => a.AIAnalysis)
                .OrderByDescending(a => a.AIAnalysis != null ? a.AIAnalysis.MatchingScore : -1)
                .Select(a => new CandidateRowViewModel
                {
                    ApplicationId = a.ApplicationId,
                    CandidateName = a.User != null ? a.User.FullName : "Unknown",
                    CandidateEmail = a.User != null ? a.User.Email : "",
                    MatchingScore = a.AIAnalysis != null ? a.AIAnalysis.MatchingScore : null,
                    Status = a.Status,
                    SubmittedAt = a.SubmittedAt
                })
                .ToListAsync();

            var selectedJob = jobs.FirstOrDefault(j => j.JobId == selectedJobId);

            var vm = new DashboardViewModel
            {
                JobId = selectedJobId,
                JobTitle = selectedJob?.Title ?? "N/A",
                Candidates = candidates,
                AllJobs = jobs
            };

            return View(vm);
        }

        // GET: /Dashboard/CandidateDetails/5
        // Shows full AI analysis for one candidate
        public async Task<IActionResult> CandidateDetails(int id)
        {
            var application = await _context.Applications
                .Include(a => a.User)
                .Include(a => a.Job)
                .Include(a => a.AIAnalysis)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
                return NotFound();

            var vm = new CandidateDetailsViewModel
            {
                ApplicationId = application.ApplicationId,
                CandidateName = application.User?.FullName ?? "Unknown",
                CandidateEmail = application.User?.Email ?? "",
                Phone = application.Phone,
                EducationLevel = application.EducationLevel,
                YearsOfExperience = application.YearsOfExperience,
                ExpectedSalary = application.ExpectedSalary,
                Status = application.Status,
                SubmittedAt = application.SubmittedAt,
                CVFileName = application.CVFileName,
                JobTitle = application.Job?.Title ?? "N/A",
                JobId = application.JobId,
                MatchingScore = application.AIAnalysis?.MatchingScore,
                Summary = application.AIAnalysis?.Summary,
                Strengths = application.AIAnalysis?.Strengths,
                Weaknesses = application.AIAnalysis?.Weaknesses,
                InterviewQuestions = application.AIAnalysis?.InterviewQuestions,
                ParsedSkills = application.AIAnalysis?.ParsedSkills,
                DetectedLanguage = application.AIAnalysis?.DetectedLanguage,
                GapReport = application.AIAnalysis?.GapReport,
                ExperienceScore = application.AIAnalysis?.ExperienceScore,
                SkillsScore = application.AIAnalysis?.SkillsScore,
                SalaryScore = application.AIAnalysis?.SalaryScore,
                EducationScore = application.AIAnalysis?.EducationScore,
            };

            return View(vm);
        }

        // POST: /Dashboard/Accept/5
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            application.Status = "Shortlisted";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { jobId = application.JobId });
        }

        // POST: /Dashboard/Reject/5
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound();

            application.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { jobId = application.JobId });
        }
        // GET: /Dashboard/Compare?jobId=1&ids=1&ids=2
public async Task<IActionResult> Compare(int jobId, List<int> ids)
{
    var job = await _context.Jobs.FindAsync(jobId);

    var allCandidates = await _context.Applications
        .Where(a => a.JobId == jobId)
        .Include(a => a.User)
        .Include(a => a.AIAnalysis)
        .OrderByDescending(a => a.AIAnalysis != null ? a.AIAnalysis.MatchingScore : -1)
        .Select(a => new CandidateRowViewModel
        {
            ApplicationId = a.ApplicationId,
            CandidateName = a.User != null ? a.User.FullName : "Unknown",
            CandidateEmail = a.User != null ? a.User.Email : "",
            MatchingScore = a.AIAnalysis != null ? a.AIAnalysis.MatchingScore : null,
            Status = a.Status,
            SubmittedAt = a.SubmittedAt
        })
        .ToListAsync();

    var selected = new List<CandidateCompareRow>();

    if (ids != null && ids.Any())
    {
        var applications = await _context.Applications
            .Where(a => ids.Contains(a.ApplicationId))
            .Include(a => a.User)
            .Include(a => a.AIAnalysis)
            .ToListAsync();

        selected = applications.Select(a => new CandidateCompareRow
        {
            ApplicationId    = a.ApplicationId,
            Name             = a.User?.FullName ?? "Unknown",
            Email            = a.User?.Email ?? "",
            MatchingScore    = a.AIAnalysis?.MatchingScore,
            ParsedSkills     = a.AIAnalysis?.ParsedSkills,
            YearsOfExperience = a.YearsOfExperience,
            ExpectedSalary   = a.ExpectedSalary,
            EducationLevel   = a.EducationLevel,
            Status           = a.Status,
            SkillsScore      = a.AIAnalysis?.SkillsScore,
            ExperienceScore  = a.AIAnalysis?.ExperienceScore,
            SalaryScore      = a.AIAnalysis?.SalaryScore,
            EducationScore   = a.AIAnalysis?.EducationScore,
        }).ToList();
    }

    var vm = new CompareViewModel
    {
        JobId         = jobId,
        JobTitle      = job?.Title ?? "N/A",
        Candidates    = selected,
        AllCandidates = allCandidates,
        SelectedIds   = ids ?? new List<int>()
    };

    return View(vm);
}
    }
}