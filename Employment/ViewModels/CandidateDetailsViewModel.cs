using Employment.Models;

namespace Employment.ViewModels
{
    public class CandidateDetailsViewModel
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public decimal ExpectedSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string CVFileName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public int JobId { get; set; }

        // AI Analysis
        public decimal? MatchingScore { get; set; }
        public string? Summary { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? InterviewQuestions { get; set; }
        public string? ParsedSkills { get; set; }
        public string? DetectedLanguage { get; set; }
        public string? GapReport { get; set; }

        // Score breakdown
        public decimal? ExperienceScore { get; set; }
        public decimal? SkillsScore { get; set; }
        public decimal? SalaryScore { get; set; }
        public decimal? EducationScore { get; set; }

        // Helper
        public List<string> InterviewQuestionsList =>
            string.IsNullOrEmpty(InterviewQuestions)
                ? new List<string>()
                : InterviewQuestions.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        public List<string> StrengthsList =>
            string.IsNullOrEmpty(Strengths)
                ? new List<string>()
                : Strengths.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        public List<string> WeaknessesList =>
            string.IsNullOrEmpty(Weaknesses)
                ? new List<string>()
                : Weaknesses.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}