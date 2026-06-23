using Employment.Data;
using Employment.Interfaces;
using Employment.Models;
using Microsoft.EntityFrameworkCore;

namespace Employment.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessAutoFilterAsync(int applicationId)
        {
           
            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application != null && application.Job != null)
            {
            
                if (application.ExpectedSalary > application.Job.SalaryMax ||
                    application.YearsOfExperience < application.Job.MinExperience)
                {
                    application.Status = "AutoRejected";
                }
                else
                {
                    application.Status = "Screening";
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}