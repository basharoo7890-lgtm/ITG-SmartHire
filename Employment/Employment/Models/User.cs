using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string Role { get; set; } = "Applicant";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Application>? Applications { get; set; }
    }
}