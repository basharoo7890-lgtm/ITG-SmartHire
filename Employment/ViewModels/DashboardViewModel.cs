using Employment.Models;

namespace Employment.ViewModels
{
    public class DashboardViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public List<CandidateRowViewModel> Candidates { get; set; } = new();
        public List<Job> AllJobs { get; set; } = new(); // for job selector dropdown
    }

    public class CandidateRowViewModel
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public decimal? MatchingScore { get; set; }
        public string? Status { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}