using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Skill_Synonym")]
    public class SkillSynonym
    {
        [Key]
        [Column("Skill_S_id")]
        public int SkillSynonymId { get; set; }

        [StringLength(100)]
        [Column("Main_Skill_Name")]
        public string? MainSkillName { get; set; }

        [StringLength(100)]
        [Column("Synonym_Name")]
        public string? SynonymName { get; set; }

        [Column("Skill_Id")]
        public int? SkillId { get; set; }

        [ForeignKey("SkillId")]
        public JobSkill? JobSkill { get; set; }
    }
}