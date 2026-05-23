using Microsoft.EntityFrameworkCore;
using Employment.Data;
using Employment.Interfaces;
using Employment.Models;
using Employment.ViewModels;

namespace Employment.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
            .Where(j => j.Status == "Active" || j.Status == "Open")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
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
    }
}