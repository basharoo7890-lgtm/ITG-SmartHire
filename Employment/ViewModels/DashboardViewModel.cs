using System.Collections.Generic;
using Employment.Models;

namespace Employment.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalJobs { get; set; }

        public int TotalApplications { get; set; }

        public List<Application>? RecentApplications { get; set; }
    }
}