namespace Employment.ViewModels
{
    public class StatisticsViewModel
    {
        public int TotalApplications { get; set; }
        public int AutoRejectedCount { get; set; }
        public double AutoRejectedPercentage { get; set; }
        public double AverageMatchingScore { get; set; }
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }
        public double AcceptanceRate { get; set; }

        public List<string> TopSkillNames { get; set; } = new();
        public List<int> TopSkillCounts { get; set; } = new();

        public List<string> JobTitles { get; set; } = new();
        public List<int> ApplicationsPerJob { get; set; } = new();
    }
}