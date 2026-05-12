namespace Employment.ViewModels
{
    public class CompareViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public List<CandidateCompareRow> Candidates { get; set; } = new();
        public List<CandidateRowViewModel> AllCandidates { get; set; } = new();
        public List<int> SelectedIds { get; set; } = new();
    }

    public class CandidateCompareRow
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal? MatchingScore { get; set; }
        public string? ParsedSkills { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ExpectedSalary { get; set; }
        public string EducationLevel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? SkillsScore { get; set; }
        public decimal? ExperienceScore { get; set; }
        public decimal? SalaryScore { get; set; }
        public decimal? EducationScore { get; set; }
    }
}