using Microsoft.AspNetCore.Mvc;
using SmartHire.Data; 
using Microsoft.EntityFrameworkCore; 

namespace SmartHire.Controllers
{
    public class JobsController : Controller
    {
        private readonly AppDbContext _context;

        public JobsController(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
           
            var activeJobs = await _context.Jobs
                .Where(j => j.Status == "Active")
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(activeJobs);
        }

    
        public async Task<IActionResult> Details(int id)
        {
         
            var job = await _context.Jobs
                .Include(j => j.JobSkills) 
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
             
                return NotFound();
            }

            return View(job);
        }
    }
}