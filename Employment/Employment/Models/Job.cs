using System;
using System.Collections.Generic;
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
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Department { get; set; }

        [Column("CreatedBy")]
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }

        public virtual ICollection<Application>? Applications { get; set; }
    }
}