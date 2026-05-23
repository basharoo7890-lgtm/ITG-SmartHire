using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.ViewModels;
using Employment.Services;
using Microsoft.AspNetCore.Authorization;



namespace Employment.Controllers
{

    // [Authorize(Roles = "HR")]  // uncomment when Identity is set up by Omar
[Authorize(Roles = "HR,Admin")]

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
               .Where(a => a.JobId == selectedJobId && a.Status != "AutoRejected")
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
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
                return NotFound();

            // Load AI analysis separately using App_Id
            var aiAnalysis = await _context.AIAnalyses
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

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
                MatchingScore = aiAnalysis?.MatchingScore,
                Summary = aiAnalysis?.Summary,
                Strengths = aiAnalysis?.Strengths,
                Weaknesses = aiAnalysis?.Weaknesses,
                InterviewQuestions = aiAnalysis?.InterviewQuestions,
                ParsedSkills = aiAnalysis?.ParsedSkills,
                DetectedLanguage = aiAnalysis?.DetectedLanguage,
                GapReport = aiAnalysis?.GapReport,
                ExperienceScore = aiAnalysis?.ExperienceScore,
                SkillsScore = aiAnalysis?.SkillsScore,
                SalaryScore = aiAnalysis?.SalaryScore,
                EducationScore = aiAnalysis?.EducationScore,
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
                    ApplicationId = a.ApplicationId,
                    Name = a.User?.FullName ?? "Unknown",
                    Email = a.User?.Email ?? "",
                    MatchingScore = a.AIAnalysis?.MatchingScore,
                    ParsedSkills = a.AIAnalysis?.ParsedSkills,
                    YearsOfExperience = a.YearsOfExperience,
                    ExpectedSalary = a.ExpectedSalary,
                    EducationLevel = a.EducationLevel,
                    Status = a.Status,
                    SkillsScore = a.AIAnalysis?.SkillsScore,
                    ExperienceScore = a.AIAnalysis?.ExperienceScore,
                    SalaryScore = a.AIAnalysis?.SalaryScore,
                    EducationScore = a.AIAnalysis?.EducationScore,
                }).ToList();
            }

            var vm = new CompareViewModel
            {
                JobId = jobId,
                JobTitle = job?.Title ?? "N/A",
                Candidates = selected,
                AllCandidates = allCandidates,
                SelectedIds = ids ?? new List<int>()
            };

            return View(vm);
        }

        // GET: /Dashboard/Statistics
        public async Task<IActionResult> Statistics()
        {
            var applications = await _context.Applications
                .Include(a => a.AIAnalysis)
                .Include(a => a.Job)
                .ToListAsync();

            var totalApplications = applications.Count;
            var autoRejected = applications.Count(a => a.Status == "AutoRejected");
            var accepted = applications.Count(a => a.Status == "Shortlisted");
            var rejected = applications.Count(a => a.Status == "Rejected");
            var pending = applications.Count(a => a.Status == "Pending");

            var scoresQuery = applications
                .Where(a => a.AIAnalysis != null && a.AIAnalysis.MatchingScore.HasValue)
                .Select(a => (double)a.AIAnalysis!.MatchingScore!.Value)
                .ToList();

            var avgScore = scoresQuery.Any() ? scoresQuery.Average() : 0;

            // Top 5 skills
            var topSkills = await _context.JobSkills
                .GroupBy(s => s.SkillName)
                .Select(g => new { Skill = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();

            // Applications per job
            var appsPerJob = applications
                .Where(a => a.Job != null)
                .GroupBy(a => a.Job!.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .ToList();

            var vm = new StatisticsViewModel
            {
                TotalApplications = totalApplications,
                AutoRejectedCount = autoRejected,
                AutoRejectedPercentage = totalApplications > 0 ? Math.Round((double)autoRejected / totalApplications * 100, 1) : 0,
                AverageMatchingScore = Math.Round(avgScore, 1),
                AcceptedCount = accepted,
                RejectedCount = rejected,
                PendingCount = pending,
                AcceptanceRate = totalApplications > 0 ? Math.Round((double)accepted / totalApplications * 100, 1) : 0,
                TopSkillNames = topSkills.Select(s => s.Skill).ToList(),
                TopSkillCounts = topSkills.Select(s => s.Count).ToList(),
                JobTitles = appsPerJob.Select(j => j.Title).ToList(),
                ApplicationsPerJob = appsPerJob.Select(j => j.Count).ToList()
            };

            return View(vm);
        }







        // GET: /Dashboard/TestParser
        public IActionResult TestParser([FromServices] CVParserService parser)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            var pdfPath = Path.Combine(uploadsPath, "15966968-32a0-48a3-a774-5c670446ab2d_doora.pdf");
            var docxPath = Path.Combine(uploadsPath, "9ffe5a5f-968e-4b2f-a487-8618932c4035_OmarAlbayyari.docx");

            var pdfText = parser.ExtractText(pdfPath);
            var docxText = parser.ExtractText(docxPath);

            return Content($"PDF ({pdfText.Length} chars):\n{pdfText.Substring(0, Math.Min(500, pdfText.Length))}\n\n---\n\nDOCX ({docxText.Length} chars):\n{docxText.Substring(0, Math.Min(500, docxText.Length))}");
        }


        // GET: /Dashboard/TestLanguage
        public IActionResult TestLanguage(
            [FromServices] CVParserService parser,
            [FromServices] LanguageDetectorService langDetector,
            [FromServices] CVCompletenessService completeness)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var docxPath = Path.Combine(uploadsPath, "9ffe5a5f-968e-4b2f-a487-8618932c4035_OmarAlbayyari.docx");

            var text = parser.ExtractText(docxPath);
            var language = langDetector.DetectLanguage(text);
            var missing = completeness.GetMissingItems(text);
            var isComplete = completeness.IsComplete(text);

            var result = $"Language: {language}\n\nComplete: {isComplete}\n\nMissing Items:\n";
            result += missing.Any() ? string.Join("\n", missing.Select(m => "- " + m)) : "Nothing missing!";

            return Content(result);
        }


        // GET: /Dashboard/TestAI/1003
        public async Task<IActionResult> TestAI(int id, [FromServices] AIAnalysisService aiService)
        {
            var result = await aiService.AnalyzeApplicationAsync(id);
            if (result == null)
                return Content("AI Analysis failed — check terminal for errors");

            return Content($"✅ AI Analysis Complete!\n\nMatching Score: {result.MatchingScore}%\nSkills Score: {result.SkillsScore}%\nExperience Score: {result.ExperienceScore}%\nSalary Score: {result.SalaryScore}%\nEducation Score: {result.EducationScore}%\n\nParsed Skills: {result.ParsedSkills}\n\nSummary: {result.Summary}\n\nStrengths:\n{result.Strengths}\n\nWeaknesses:\n{result.Weaknesses}\n\nInterview Questions:\n{result.InterviewQuestions}");
        }

        // GET: /Dashboard/TestGapReport/1003
        public async Task<IActionResult> TestGapReport(int id, [FromServices] SkillsGapService gapService)
        {
            var result = await gapService.GenerateGapReportAsync(id);
            if (result == null)
                return Content("Gap report generation failed");
            return Content(result);
        }


        // GET: /Dashboard/AIRecommendation?jobId=6
        public async Task<IActionResult> AIRecommendation(int jobId, [FromServices] GeminiService gemini)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) return NotFound();

            var candidates = await _context.Applications
                .Where(a => a.JobId == jobId && a.Status != "AutoRejected")
                .Include(a => a.User)
                .Include(a => a.AIAnalysis)
                .ToListAsync();

            if (!candidates.Any())
                return Content("No candidates found for this job.");

            // Build comparison prompt
            var candidatesSummary = string.Join("\n\n", candidates.Select((c, i) =>
                $"Candidate {i + 1}: {c.User?.FullName}\n" +
                $"- Matching Score: {c.AIAnalysis?.MatchingScore}%\n" +
                $"- Skills: {c.AIAnalysis?.ParsedSkills}\n" +
                $"- Experience: {c.YearsOfExperience} years\n" +
                $"- Education: {c.EducationLevel}\n" +
                $"- Expected Salary: ${c.ExpectedSalary}\n" +
                $"- Strengths: {c.AIAnalysis?.Strengths}\n" +
                $"- Weaknesses: {c.AIAnalysis?.Weaknesses}"
            ));

            var prompt = $"""
        You are an expert HR assistant. Analyze these candidates for the position of {job.Title} and recommend the best one.

        Job Requirements:
        - Required Education: {job.RequiredEducation}
        - Min Experience: {job.MinExperience} years
        - Salary Range: ${job.SalaryMin} - ${job.SalaryMax}
        - Department: {job.Department}

        Candidates:
        {candidatesSummary}

        Please:
        1. Rank all candidates from best to worst
        2. Explain why the top candidate is the best fit
        3. Mention any concerns about each candidate
        4. Give a final recommendation

        Be professional and specific.
        """;

            var aiRecommendation = await gemini.GenerateAsync(prompt);

            var vm = new AIRecommendationViewModel
            {
                JobId = jobId,
                JobTitle = job.Title,
                Recommendation = aiRecommendation ?? "AI recommendation failed. Please try again.",
                Candidates = candidates.Select(c => new CandidateRowViewModel
                {
                    ApplicationId = c.ApplicationId,
                    CandidateName = c.User?.FullName ?? "Unknown",
                    CandidateEmail = c.User?.Email ?? "",
                    MatchingScore = c.AIAnalysis?.MatchingScore,
                    Status = c.Status,
                    SubmittedAt = c.SubmittedAt
                }).OrderByDescending(c => c.MatchingScore).ToList()
            };

            return View(vm);
        }

    }
}