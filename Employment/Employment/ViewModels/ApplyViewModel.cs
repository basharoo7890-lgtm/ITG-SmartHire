using System.ComponentModel.DataAnnotations;

namespace Employment.ViewModels
{
    public class ApplyViewModel
    {
        [Required]
        public int JobId { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Range(0, 1000000)]
        public decimal? ExpectedSalary { get; set; }

        [Range(0, 50)]
        public int? YearsOfExperience { get; set; }

        [StringLength(100)]
        public string? EducationLevel { get; set; }

        [StringLength(255)]
        public string? CVFileName { get; set; }

        public string? CVText { get; set; }
    }
}