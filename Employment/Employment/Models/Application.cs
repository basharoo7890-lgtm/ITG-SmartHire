using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Employment.Models;

namespace Employment.Models
{
    [Table("Applicant")]
    public class Application
    {
        [Key]
        [Column("AppId")]
        public int ApplicationId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExpectedSalary { get; set; }

        public int? YearsOfExperience { get; set; }

        [StringLength(100)]
        public string? EducationLevel { get; set; }

        [StringLength(255)]
        public string? CVFileName { get; set; }

        public string? CVText { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
        public AIAnalysis? AIAnalysis { get; set; }

        public DateTime SubmittedAt { get; set; }

[ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("JobId")]
    public Job? Job { get; set; }
}
}