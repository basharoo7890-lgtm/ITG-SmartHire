using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Job_Skill")]
    public class JobSkill
    {
        [Key]
        [Column("Skill_Id")]
        public int SkillId { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        [StringLength(100)]
        public string SkillName { get; set; } = string.Empty;

        [Column("MinYearsOfExperience")]
        public int? MinYearOfExperience { get; set; }

        public int? ImportantLevel { get; set; }

        public bool IsRequired { get; set; } = true;
    }
}