namespace Employment.ViewModels
{
    public class AIRecommendationViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public List<CandidateRowViewModel> Candidates { get; set; } = new();
    }
}