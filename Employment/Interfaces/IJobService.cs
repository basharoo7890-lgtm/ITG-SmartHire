using System.Collections.Generic;
using System.Threading.Tasks;
using Employment.Models;
using Employment.ViewModels;

namespace Employment.Interfaces
{
    public interface IJobService
    {
        Task<List<Job>> GetAllJobsAsync();

        Task<Job?> GetJobByIdAsync(int jobId);

        Task<JobDetailsViewModel?> GetJobDetailsAsync(int jobId);
    }
}