using Microsoft.AspNetCore.Mvc;
using Employment.Interfaces;

namespace Employment.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _jobService.GetJobDetailsAsync(id);
            if (viewModel == null) return NotFound();
            return View(viewModel);
        }
    }
}