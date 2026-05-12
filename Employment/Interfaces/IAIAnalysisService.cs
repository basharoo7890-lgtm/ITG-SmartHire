using Employment.Models;
using Employment.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Employment.Interfaces
{
    public interface IAIAnalysisService
    {
        Task<AIAnalysis?> GetAnalysisByApplicationIdAsync(int applicationId);

        Task<List<AIAnalysis>> GetAllAnalysesAsync();

        Task<AIAnalysis?> GenerateAnalysisAsync(int applicationId);

        Task<decimal?> CalculateMatchingScoreAsync(int applicationId);

        Task<List<string>?> GenerateInterviewQuestionsAsync(int applicationId);

        Task<bool> SubmitApplicationAsync(ApplyViewModel model, string cvFileName); 
        Task<IEnumerable<Application>> GetUserApplicationsAsync(int userId);
        Task<bool> ProcessAutoFilterAsync(int applicationId); 
    }
}