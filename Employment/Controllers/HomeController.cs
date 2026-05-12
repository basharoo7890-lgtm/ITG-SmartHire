using Employment.Models;
using Employment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Employment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobService _jobService;

        public HomeController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Index()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return View(jobs);
        }

       
        public async Task<IActionResult> JobDetails(int? id)
        {
           
            if (id == null)
            {
                return View();
            }

            
            var viewModel = await _jobService.GetJobDetailsAsync(id.Value);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        public IActionResult OpenPositions()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}