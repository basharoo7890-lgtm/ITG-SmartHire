using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("Descreption")]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMax { get; set; }

        [StringLength(100)]
        public string? RequiredEducation { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? MinExperience { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}