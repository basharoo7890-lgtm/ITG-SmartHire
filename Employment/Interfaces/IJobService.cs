using System.Collections.Generic;
using System.Threading.Tasks;
using Employment.Models;
using Employment.ViewModels;

namespace Employment.Interfaces
{
  // In IJobService.cs
public interface IJobService
{
    Task<List<Job>> GetAllJobsAsync();
    Task<Job?> GetJobByIdAsync(int jobId);
    Task<JobDetailsViewModel?> GetJobDetailsAsync(int jobId);
    
    // Add this new method
    Task<bool> HasUserAppliedToJobAsync(int userId, int jobId);
    Task<Dictionary<int, bool>> GetUserApplicationsStatusAsync(int userId, List<int> jobIds);
}
    
}