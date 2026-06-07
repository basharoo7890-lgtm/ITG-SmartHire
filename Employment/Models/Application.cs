using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Application")]
    public class Application
    {
        [Key]
        [Column("AppId")]
        public int ApplicationId { get; set; }

        public int JobId { get; set; }
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public decimal ExpectedSalary { get; set; }
        public int YearsOfExperience { get; set; }

        [Required]
        [StringLength(100)]
        public string EducationLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string CVFileName { get; set; } = string.Empty;

        [Required]
        public string CVText { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("JobId")]
        public virtual Job? Job { get; set; }

        // ✅ أضف هذا السطر
        public virtual AIAnalysis? AIAnalysis { get; set; }
    }
}