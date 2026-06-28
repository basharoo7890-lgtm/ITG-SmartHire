using Employment.Data;
using Employment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Employment.Services
{
    public class AIAnalysisService
    {
        private readonly GeminiService _gemini;
        private readonly CVParserService _cvParser;
        private readonly LanguageDetectorService _langDetector;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AIAnalysisService> _logger;

        public AIAnalysisService(
            GeminiService gemini,
            CVParserService cvParser,
            LanguageDetectorService langDetector,
            ApplicationDbContext context,
            ILogger<AIAnalysisService> logger)
        {
            _gemini = gemini;
            _cvParser = cvParser;
            _langDetector = langDetector;
            _context = context;
            _logger = logger;
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
            var cvPath = Path.Combine(uploadsPath, application.CVFileName ?? "");
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
                .Select(s => s.SkillName ?? "")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToListAsync();

            var skillsList = jobSkills.Any() ? string.Join(", ", jobSkills) : "general skills";

            // Step 4 - Extract skills from CV
            var jsonFormat = "{\"parsedSkills\": \"skill1, skill2\", \"summary\": \"summary here\", \"strengths\": \"strength1\\nstrength2\", \"weaknesses\": \"weakness1\\nweakness2\", \"interviewQuestions\": \"Q1?\\nQ2?\\nQ3?\\nQ4?\\nQ5?\"}";

            var extractPrompt = $"{languageInstruction}\n\nCV Text:\n{cvText}\n\nJob Title: {application.Job.Title}\nRequired Skills: {skillsList}\n\nRespond ONLY with a JSON object in this exact format, no markdown:\n{jsonFormat}";

            const int maxParseAttempts = 3;
            string? aiResponse = null;
            System.Text.Json.JsonDocument? doc = null;
            string parsedSkills = "", summary = "", strengths = "", weaknesses = "", interviewQuestions = "";
            bool parsedOk = false;

            for (int attempt = 1; attempt <= maxParseAttempts && !parsedOk; attempt++)
            {
                aiResponse = await _gemini.GenerateAsync(extractPrompt);
                _logger.LogDebug("[AI Raw Response - attempt {Attempt}]: {Preview}", attempt, aiResponse?.Substring(0, Math.Min(500, aiResponse?.Length ?? 0)));

                if (aiResponse == null)
                {
                    _logger.LogWarning("[AI Analysis] Attempt {Attempt}/{Max}: Gemini/OpenRouter returned null", attempt, maxParseAttempts);
                    continue;
                }

                try
                {
                    var json = aiResponse.Trim();
                    if (json.StartsWith("```"))
                        json = string.Join("\n", json.Split('\n').Skip(1).SkipLast(1));

                    json = System.Text.RegularExpressions.Regex.Replace(
                    json, @"\\(?![""\\/bfnrtu])", "");
                    doc = System.Text.Json.JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    parsedSkills = root.GetProperty("parsedSkills").GetString() ?? "";
                    summary = root.GetProperty("summary").GetString() ?? "";
                    strengths = root.GetProperty("strengths").GetString() ?? "";
                    weaknesses = root.GetProperty("weaknesses").GetString() ?? "";
                    interviewQuestions = root.GetProperty("interviewQuestions").GetString() ?? "";

                    parsedOk = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[AI Analysis] Attempt {Attempt}/{Max}: failed to parse AI JSON response. Raw: {RawResponse}", attempt, maxParseAttempts, aiResponse);
                }
            }

            if (!parsedOk)
            {
                _logger.LogError("[AI Analysis] All {Max} attempts failed for application {ApplicationId}", maxParseAttempts, applicationId);
                return null;
            }

            try
            {
                // Step 6 - Calculate scores
                var scores = await CalculateScoresAsync(application, parsedSkills, jobSkills);

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

                    // Update application status (no auto-rejection)
                    await UpdateApplicationStatusAsync(applicationId, scores.MatchingScore);

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

                    // Update application status (no auto-rejection)
                    await UpdateApplicationStatusAsync(applicationId, scores.MatchingScore);

                    return analysis;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Analysis Error. Raw response: {RawResponse}", aiResponse);
                return null;
            }
        }

        private async Task<(decimal MatchingScore, decimal SkillsScore, decimal ExperienceScore, decimal SalaryScore, decimal EducationScore)>
            CalculateScoresAsync(Application app, string parsedSkills, List<string> jobSkills)
        {
            // Read weights from System_Setting table
            var settings = await _context.SystemSettings.ToListAsync();

            decimal skillsWeight = GetSettingValue(settings, "SkillsWeight", 40) / 100m;
            decimal experienceWeight = GetSettingValue(settings, "ExperienceWeight", 25) / 100m;
            decimal salaryWeight = GetSettingValue(settings, "SalaryWeight", 20) / 100m;
            decimal educationWeight = GetSettingValue(settings, "EducationWeight", 15) / 100m;

            _logger.LogDebug("[Weights] Skills:{S}% Experience:{E}% Salary:{Sal}% Education:{Ed}%", skillsWeight * 100, experienceWeight * 100, salaryWeight * 100, educationWeight * 100);

            // Calculate scores
            var skillsScore = await CalculateSkillsScoreAsync(app, parsedSkills, jobSkills);
            var experienceScore = CalculateExperienceScore(app);
            var salaryScore = CalculateSalaryScore(app, settings);
            var educationScore = CalculateEducationScore(app);

            // Weighted total
            var matchingScore = (skillsScore * skillsWeight) +
                                (experienceScore * experienceWeight) +
                                (salaryScore * salaryWeight) +
                                (educationScore * educationWeight);

            return (
                Math.Round(matchingScore, 2),
                Math.Round(skillsScore, 2),
                Math.Round(experienceScore, 2),
                Math.Round(salaryScore, 2),
                Math.Round(educationScore, 2)
            );
        }

        private async Task<decimal> CalculateSkillsScoreAsync(Application app, string parsedSkills, List<string> jobSkills)
        {
            decimal skillsScore = 0;

            if (jobSkills.Any() && !string.IsNullOrEmpty(parsedSkills))
            {
                var candidateSkills = parsedSkills.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                var synonyms = await _context.SkillSynonyms.ToListAsync();
                
                var validSynonymPairs = new List<(string MainSkill, string Synonym)>();
                foreach (var synonym in synonyms)
                {
                    if (!string.IsNullOrEmpty(synonym.MainSkillName) && !string.IsNullOrEmpty(synonym.SynonymName))
                    {
                        validSynonymPairs.Add((synonym.MainSkillName.ToLower(), synonym.SynonymName.ToLower()));
                    }
                }

                int matched = 0;
                foreach (var jobSkill in jobSkills)
                {
                    var jobSkillLower = jobSkill.ToLower().Trim();

                    bool isMatched = candidateSkills.Any(cs =>
                        cs == jobSkillLower ||
                        cs.StartsWith(jobSkillLower) ||
                        jobSkillLower.StartsWith(cs));

                    if (!isMatched && validSynonymPairs.Any())
                    {
                        var jobSkillSynonyms = new List<string>();
                        
                        var mainSkillMatches = validSynonymPairs
                            .Where(p => p.MainSkill == jobSkillLower)
                            .Select(p => p.Synonym)
                            .ToList();
                        jobSkillSynonyms.AddRange(mainSkillMatches);
                        
                        var synonymMatches = validSynonymPairs
                            .Where(p => p.Synonym == jobSkillLower)
                            .Select(p => p.MainSkill)
                            .ToList();
                        jobSkillSynonyms.AddRange(synonymMatches);
                        
                        jobSkillSynonyms.Add(jobSkillLower);
                        jobSkillSynonyms = jobSkillSynonyms.Distinct().ToList();

                        isMatched = candidateSkills.Any(cs => jobSkillSynonyms.Contains(cs));
                    }

                    if (isMatched) matched++;
                    _logger.LogDebug("[Skills] {Skill}: {Status}", jobSkill, isMatched ? "Matched" : "Not Matched");
                }

                skillsScore = (decimal)matched / jobSkills.Count * 100;
                _logger.LogDebug("[Skills] Score: {Matched}/{Total} = {Score}%", matched, jobSkills.Count, skillsScore);
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

            return Math.Min(skillsScore, 100);
        }

        private decimal CalculateExperienceScore(Application app)
        {
            decimal experienceScore = 0;

            if (app.Job?.MinExperience > 0)
            {
                var diff = app.YearsOfExperience - app.Job.MinExperience;
                if (diff >= 0) experienceScore = 100;
                else if (diff == -1) experienceScore = 70;
                else if (diff == -2) experienceScore = 40;
                else experienceScore = 20;
            }
            else if (app.YearsOfExperience == 0)
            {
                experienceScore = 40;
            }
            else
            {
                experienceScore = 75;
            }

            _logger.LogDebug("[Experience] Candidate: {CandidateYears} years, Required: {RequiredYears} years => Score: {Score}", app.YearsOfExperience, app.Job?.MinExperience ?? 0, experienceScore);
            return experienceScore;
        }

        /// <summary>
        /// Calculate salary score - PENALTY for exceeding budget, but NO auto-rejection
        /// Score decreases progressively as expected salary goes above max
        /// </summary>
        private decimal CalculateSalaryScore(Application app, List<SystemSetting> settings)
        {
            bool hasSalaryMax = app.Job?.SalaryMax.HasValue == true && app.Job.SalaryMax > 0;
            
            // If no salary max defined, give full score
            if (!hasSalaryMax)
            {
                _logger.LogDebug("[Salary] No salary max defined for this job => Score: 100");
                return 100;
            }

            var salaryMax = app.Job!.SalaryMax!.Value;
            var expectedSalary = app.ExpectedSalary;
            
            _logger.LogDebug("[Salary] Job Max: {SalaryMax}, Candidate Expected: {Expected}", salaryMax, expectedSalary);
            
            // Case 1: Expected salary is within budget - FULL SCORE
            if (expectedSalary <= salaryMax)
            {
                var percentageOfMax = (expectedSalary / salaryMax) * 100;
                _logger.LogDebug("[Salary] Within budget ({Pct:F0}% of max) => Score: 100", percentageOfMax);
                return 100;
            }
            
            // Case 2: Expected salary exceeds budget - CALCULATE PENALTY
            var excessAmount = expectedSalary - salaryMax;
            var excessPercentage = (excessAmount / salaryMax) * 100;
            
            _logger.LogDebug("[Salary] Exceeds budget by {ExcessAmount} ({ExcessPct:F1}%)", excessAmount, excessPercentage);
            
            // Progressive penalty system - the more over budget, the lower the score
            // But NEVER goes to 0 completely (minimum 10%)
            
            if (excessPercentage <= 10)
            {
                // Up to 10% over budget: 90% score
                _logger.LogDebug("[Salary] Slightly over budget (<=10%) => Score: 90");
                return 90;
            }
            else if (excessPercentage <= 20)
            {
                // 11-20% over budget: 75% score
                _logger.LogDebug("[Salary] Moderately over budget (11-20%) => Score: 75");
                return 75;
            }
            else if (excessPercentage <= 30)
            {
                // 21-30% over budget: 60% score
                _logger.LogDebug("[Salary] Significantly over budget (21-30%) => Score: 60");
                return 60;
            }
            else if (excessPercentage <= 50)
            {
                // 31-50% over budget: 40% score
                _logger.LogDebug("[Salary] Well over budget (31-50%) => Score: 40");
                return 40;
            }
            else if (excessPercentage <= 75)
            {
                // 51-75% over budget: 25% score
                _logger.LogDebug("[Salary] Far over budget (51-75%) => Score: 25");
                return 25;
            }
            else
            {
                // Over 75% over budget: 10% score (minimum, not zero)
                _logger.LogDebug("[Salary] Extremely over budget (>75%) => Score: 10");
                return 10;
            }
        }

        private decimal CalculateEducationScore(Application app)
        {
            decimal educationScore = 50;
            
            var eduMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "high school", 1 },
                { "diploma", 2 },
                { "bachelor", 3 },
                { "master", 4 },
                { "phd", 5 }
            };
            
            var candidateEdu = eduMap.FirstOrDefault(e => (app.EducationLevel ?? "").ToLower().Contains(e.Key)).Value;
            var requiredEdu = eduMap.FirstOrDefault(e => (app.Job?.RequiredEducation ?? "").ToLower().Contains(e.Key)).Value;

            if (candidateEdu > 0 && requiredEdu > 0)
            {
                if (candidateEdu >= requiredEdu)
                {
                    educationScore = 100;
                    _logger.LogDebug("[Education] Candidate: {CandEdu}, Required: {ReqEdu} => Meets requirement. Score: 100", app.EducationLevel, app.Job?.RequiredEducation);
                }
                else if (candidateEdu == requiredEdu - 1)
                {
                    educationScore = 50;
                    _logger.LogDebug("[Education] Candidate: {CandEdu}, Required: {ReqEdu} => One level below. Score: 50", app.EducationLevel, app.Job?.RequiredEducation);
                }
                else
                {
                    educationScore = 20;
                    _logger.LogDebug("[Education] Candidate: {CandEdu}, Required: {ReqEdu} => Multiple levels below. Score: 20", app.EducationLevel, app.Job?.RequiredEducation);
                }
            }
            else
            {
                _logger.LogDebug("[Education] Candidate: {CandEdu}, Required: {ReqEdu} => Default Score: 50", app.EducationLevel, app.Job?.RequiredEducation ?? "None");
            }

            return educationScore;
        }

        private decimal GetSettingValue(List<SystemSetting> settings, string key, decimal defaultValue)
        {
            var setting = settings.FirstOrDefault(s => s.SettingKey == key);
            if (setting != null && decimal.TryParse(setting.SettingValue, out var value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Updates application status - NO AUTO-REJECTION EVER
        /// All applicants keep "Screening" status by default
        /// </summary>
        private async Task UpdateApplicationStatusAsync(int applicationId, decimal matchingScore)
        {
            try
            {
                var application = await _context.Applications.FindAsync(applicationId);
                if (application == null) return;

                // CRITICAL: NEVER change status if it's already been manually set by HR
                if (application.Status == "Rejected" || 
                    application.Status == "Shortlisted" || 
                    application.Status == "Hired" ||
                    application.Status == "Interviewing")
                {
                    _logger.LogInformation("[AI Pipeline] Preserving manual status '{Status}' for application {ApplicationId}", application.Status, applicationId);
                    return;
                }

                // Keep status as "Screening" - NO AUTO-REJECTION
                // Only HR can change status
                _logger.LogInformation("[AI Pipeline] Application {ApplicationId} status is 'Screening' (Match Score: {Score}%)", applicationId, matchingScore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AI Pipeline] Error updating status for application {ApplicationId}", applicationId);
            }
        }
    }
}