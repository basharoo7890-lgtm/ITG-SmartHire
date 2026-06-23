using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.Interfaces;
using Employment.Models;
using Employment.ViewModels;
using Microsoft.Extensions.Logging; // Add this

namespace Employment.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobService> _logger; // Add logger

        public JobService(ApplicationDbContext context, ILogger<JobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            var allJobs = await _context.Jobs
                .AsNoTracking()
                .ToListAsync();

            var activeJobs = allJobs
                .Where(j => j.Status == "Active")
                .OrderByDescending(j => j.CreatedAt)
                .ToList();

            return activeJobs;
        }

        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs.FindAsync(jobId);
        }

        public async Task<JobDetailsViewModel?> GetJobDetailsAsync(int jobId)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) return null;

            var skills = await _context.JobSkills
                .Where(s => s.JobId == jobId)
                .ToListAsync();

            return new JobDetailsViewModel { Job = job, JobSkills = skills };
        }
        // In JobService.cs
public async Task<bool> HasUserAppliedToJobAsync(int userId, int jobId)
{
    return await _context.Applications
        .AnyAsync(a => a.UserId == userId && a.JobId == jobId);
}

public async Task<Dictionary<int, bool>> GetUserApplicationsStatusAsync(int userId, List<int> jobIds)
{
    var appliedJobIds = await _context.Applications
        .Where(a => a.UserId == userId && jobIds.Contains(a.JobId))
        .Select(a => a.JobId)
        .ToListAsync();
    
    return jobIds.ToDictionary(jobId => jobId, jobId => appliedJobIds.Contains(jobId));
}
    }
}