using System.Collections.Generic;
using Employment.Models;

namespace Employment.ViewModels
{
    public class JobDetailsViewModel
    {
        public Job? Job { get; set; }

        public List<JobSkill>? JobSkills { get; set; }
    }
}