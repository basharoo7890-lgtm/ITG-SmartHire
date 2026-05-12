using System.ComponentModel.DataAnnotations;

namespace Employment.ViewModels
{
    public class ApplyViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDepartment { get; set; } = string.Empty;
        public string JobLocation { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public decimal ExpectedSalary { get; set; }

        [Required]
        public int YearsOfExperience { get; set; }

        [Required]
        public string EducationLevel { get; set; } = string.Empty;

        public IFormFile? CVFile { get; set; }
    }
}
