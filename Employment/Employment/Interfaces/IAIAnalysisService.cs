using System.Collections.Generic;
using System.Threading.Tasks;
using Employment.Models;

namespace Employment.Interfaces
{
    public interface IAIAnalysisService
    {
        Task<AIAnalysis?> GetAnalysisByApplicationIdAsync(int applicationId);

        Task<List<AIAnalysis>> GetAllAnalysesAsync();

        Task<AIAnalysis?> GenerateAnalysisAsync(int applicationId);

        Task<decimal?> CalculateMatchingScoreAsync(int applicationId);

        Task<List<string>?> GenerateInterviewQuestionsAsync(int applicationId);
    }
}