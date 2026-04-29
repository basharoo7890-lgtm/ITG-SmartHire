using System.Collections.Generic;
using Employment.Models;

namespace Employment.ViewModels
{
    public class CandidateCompareViewModel
    {
        public int JobId { get; set; }

        public List<Application>? Applications { get; set; }

        public List<AIAnalysis>? AIAnalyses { get; set; }
    }
}