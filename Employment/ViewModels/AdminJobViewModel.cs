using System.ComponentModel.DataAnnotations;

namespace Employment.ViewModels
{
    public class AdminJobViewModel
    {
        public int JobId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int MinExperience { get; set; } = 0;

        [Required]
        public string RequiredEducation { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public List<JobSkillViewModel> Skills { get; set; } = new();
    }

    public class JobSkillViewModel
    {
        public string SkillName { get; set; } = string.Empty;
        public int MinYearsOfExperience { get; set; } = 0;
        public string ImportantLevel { get; set; } = "Required";
        public bool IsRequired { get; set; } = true;
    }
}