using Microsoft.AspNetCore.Mvc;

namespace Employment.Controllers
{
    public class ApplicantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Apply(string title, string department, string location, string type)
        {
            ViewBag.JobTitle = title;
            ViewBag.Department = department;
            ViewBag.Location = location;
            ViewBag.JobType = type;

            return View();
        }
    }
}
