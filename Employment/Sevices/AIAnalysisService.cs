using Employment.Data;
using Employment.Models;
using Microsoft.EntityFrameworkCore;

namespace Employment.Services
{
    public class AIAnalysisService
    {
        private readonly GeminiService _gemini;
        private readonly CVParserService _cvParser;
        private readonly LanguageDetectorService _langDetector;
        private readonly ApplicationDbContext _context;

        public AIAnalysisService(
            GeminiService gemini,
            CVParserService cvParser,
            LanguageDetectorService langDetector,
            ApplicationDbContext context)
        {
            _gemini = gemini;
            _cvParser = cvParser;
            _langDetector = langDetector;
            _context = context;
        }

        public async Task<AIAnalysis?> AnalyzeApplicationAsync(int applicationId)
        {
            var application = await _context.Applications
    .Include(a => a.Job)
    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null || application.Job == null)
                return null;

            // Step 1 - Parse CV
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var cvPath = Path.Combine(uploadsPath, application.CVFileName);
            var cvText = _cvParser.ExtractText(cvPath);

            if (string.IsNullOrEmpty(cvText))
                cvText = application.CVText ?? "";

            // Step 2 - Detect language
            var language = _langDetector.DetectLanguage(cvText);

            // Step 3 - Build language-aware prompt
            var languageInstruction = language switch
            {
                "arabic" => "The CV is in Arabic. Extract all information and respond in English JSON.",
                "mixed" => "The CV contains Arabic and English. Extract all info and respond in English JSON.",
                _ => "Extract from this CV and respond in English JSON."
            };

            var jobSkills = await _context.JobSkills
     .Where(s => s.JobId == application.Job.JobId)
     .Select(s => s.SkillName)
     .ToListAsync();

            var skillsList = jobSkills.Any() ? string.Join(", ", jobSkills) : "general skills";

            // Step 4 - Extract skills from CV
            var jsonFormat = "{\"parsedSkills\": \"skill1, skill2\", \"summary\": \"summary here\", \"strengths\": \"strength1\\nstrength2\", \"weaknesses\": \"weakness1\\nweakness2\", \"interviewQuestions\": \"Q1?\\nQ2?\\nQ3?\\nQ4?\\nQ5?\"}";

            var extractPrompt = $"{languageInstruction}\n\nCV Text:\n{cvText}\n\nJob Title: {application.Job.Title}\nRequired Skills: {skillsList}\n\nRespond ONLY with a JSON object in this exact format, no markdown:\n{jsonFormat}";

        var aiResponse = await _gemini.GenerateAsync(extractPrompt);
Console.WriteLine($"[AI Raw Response]: {aiResponse?.Substring(0, Math.Min(500, aiResponse?.Length ?? 0))}");

if (aiResponse == null)
                return null;

            // Step 5 - Parse JSON response
            try
            {
                // Clean response
                var json = aiResponse.Trim();
                if (json.StartsWith("```"))
                    json = string.Join("\n", json.Split('\n').Skip(1).SkipLast(1));

                var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                var parsedSkills = root.GetProperty("parsedSkills").GetString() ?? "";
                var summary = root.GetProperty("summary").GetString() ?? "";
                var strengths = root.GetProperty("strengths").GetString() ?? "";
                var weaknesses = root.GetProperty("weaknesses").GetString() ?? "";
                var interviewQuestions = root.GetProperty("interviewQuestions").GetString() ?? "";

                // Step 6 - Calculate scores
                var scores = CalculateScores(application, parsedSkills, jobSkills);

                // Step 7 - Save to database
                var existing = await _context.AIAnalyses
                    .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                if (existing != null)
                {
                    existing.MatchingScore = scores.MatchingScore;
                    existing.ParsedSkills = parsedSkills;
                    existing.Summary = summary;
                    existing.Strengths = strengths;
                    existing.Weaknesses = weaknesses;
                    existing.InterviewQuestions = interviewQuestions;
                    existing.SkillsScore = scores.SkillsScore;
                    existing.ExperienceScore = scores.ExperienceScore;
                    existing.SalaryScore = scores.SalaryScore;
                    existing.EducationScore = scores.EducationScore;
                    existing.AnalysisDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Auto shortlist/reject based on score
                    await AutoUpdateApplicationStatusAsync(applicationId, scores.MatchingScore);

                    return existing;
                }
                else
                {
                    var analysis = new AIAnalysis
                    {
                        ApplicationId = applicationId,
                        MatchingScore = scores.MatchingScore,
                        ParsedSkills = parsedSkills,
                        Summary = summary,
                        Strengths = strengths,
                        Weaknesses = weaknesses,
                        InterviewQuestions = interviewQuestions,
                        SkillsScore = scores.SkillsScore,
                        ExperienceScore = scores.ExperienceScore,
                        SalaryScore = scores.SalaryScore,
                        EducationScore = scores.EducationScore,
                        AnalysisDate = DateTime.Now
                    };
                    _context.AIAnalyses.Add(analysis);
                    await _context.SaveChangesAsync();

                    // Auto shortlist/reject based on score
                    await AutoUpdateApplicationStatusAsync(applicationId, scores.MatchingScore);

                    return analysis;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Analysis Error: {ex.Message}");
                Console.WriteLine($"Raw response: {aiResponse}");
                return null;
            }
        }

        private (decimal MatchingScore, decimal SkillsScore, decimal ExperienceScore, decimal SalaryScore, decimal EducationScore)
            CalculateScores(Application app, string parsedSkills, List<string> jobSkills)
        {
                Console.WriteLine($"[CalculateScores] ParsedSkills: {parsedSkills}");
    Console.WriteLine($"[CalculateScores] JobSkills: {string.Join(", ", jobSkills)}");

        
// Skills score (40%)
decimal skillsScore = 0;
if (jobSkills.Any() && !string.IsNullOrEmpty(parsedSkills))
{
    var candidateSkills = parsedSkills.ToLower()
        .Split(',')
        .Select(s => s.Trim())
        .ToList();

    int matched = 0;
    foreach (var jobSkill in jobSkills)
    {
        var jobSkillLower = jobSkill.ToLower().Trim();
        bool isMatched = candidateSkills.Any(cs => 
            cs == jobSkillLower || 
            cs.StartsWith(jobSkillLower) || 
            jobSkillLower.StartsWith(cs));
        
        if (isMatched) matched++;
        Console.WriteLine($"[Skills] {jobSkill}: {(isMatched ? "✅ matched" : "❌ not found")}");
    }

    skillsScore = (decimal)matched / jobSkills.Count * 100;
    Console.WriteLine($"[Skills] Score: {matched}/{jobSkills.Count} = {skillsScore}%");
}
else if (!jobSkills.Any() && !string.IsNullOrEmpty(parsedSkills))
{
    var candidateSkillCount = parsedSkills.Split(',').Length;
    skillsScore = Math.Min(candidateSkillCount * 8, 65);
}
else
{
    skillsScore = 30;
}

            
         // Experience score (25%)
decimal experienceScore = 0;
if (app.Job?.MinExperience > 0)
{
    var ratio = (decimal)app.YearsOfExperience / app.Job.MinExperience;
    experienceScore = Math.Min(ratio * 100, 100);
    // Penalize if overqualified by a lot
    if (ratio > 3) experienceScore = 85;
}
else if (app.YearsOfExperience == 0)
{
    experienceScore = 40; // entry level gets lower score
}
else
{
    experienceScore = 75;
}

            // Salary score (20%)
            decimal salaryScore = 100;
            if (app.Job?.SalaryMax.HasValue == true && app.Job.SalaryMax > 0)
            {
                if (app.ExpectedSalary <= app.Job.SalaryMax)
                    salaryScore = 100;
                else
                    salaryScore = Math.Max(0, 100 - ((app.ExpectedSalary - app.Job.SalaryMax.Value) / app.Job.SalaryMax.Value * 100));
            }

            // Education score (15%)
            decimal educationScore = 50;
            var eduMap = new Dictionary<string, int>
            {
                { "high school", 1 }, { "diploma", 2 }, { "bachelor", 3 },
                { "master", 4 }, { "phd", 5 }
            };
            var candidateEdu = eduMap.FirstOrDefault(e => app.EducationLevel.ToLower().Contains(e.Key)).Value;
            var requiredEdu = eduMap.FirstOrDefault(e => (app.Job?.RequiredEducation ?? "").ToLower().Contains(e.Key)).Value;
            if (candidateEdu > 0 && requiredEdu > 0)
                educationScore = candidateEdu >= requiredEdu ? 100 : (decimal)candidateEdu / requiredEdu * 100;

            // Weighted total
            var matchingScore = (skillsScore * 0.40m) + (experienceScore * 0.25m) + (salaryScore * 0.20m) + (educationScore * 0.15m);

            return (
                Math.Round(matchingScore, 2),
                Math.Round(skillsScore, 2),
                Math.Round(experienceScore, 2),
                Math.Round(salaryScore, 2),
                Math.Round(educationScore, 2)
            );
        }
        private async Task AutoUpdateApplicationStatusAsync(int applicationId, decimal matchingScore)
{
    try
    {
        // Get thresholds from System_Setting
        var shortlistThresholdSetting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "AutoShortlistThreshold");
        var rejectThresholdSetting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "AutoRejectThreshold");

        var shortlistThreshold = decimal.TryParse(shortlistThresholdSetting?.SettingValue, out var st) ? st : 70;
        var rejectThreshold = decimal.TryParse(rejectThresholdSetting?.SettingValue, out var rt) ? rt : 50;

        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null) return;

        // Don't override if already manually decided
       if (application.Status == "Rejected")
    return;

        if (matchingScore >= shortlistThreshold)
        {
            application.Status = "Shortlisted";
            Console.WriteLine($"[AI Pipeline] ✅ Auto-shortlisted application {applicationId} (score: {matchingScore}%)");
        }
        else if (matchingScore < rejectThreshold)
        {
            application.Status = "AutoRejected";
            Console.WriteLine($"[AI Pipeline] ❌ Auto-rejected application {applicationId} (score: {matchingScore}%)");
        }
        else
        {
            application.Status = "Screening";
            Console.WriteLine($"[AI Pipeline] ⏳ Application {applicationId} needs manual review (score: {matchingScore}%)");
        }

        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AI Pipeline] ❌ Auto-status error: {ex.Message}");
    }
}
    }
}