using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Skill_Synonyms")]
    public class SkillSynonym
    {
        [Key]
        public int SkillSynonymId { get; set; }

        [StringLength(100)]
        [Column("Main_Skill_Name")]
        public string? MainSkillName { get; set; }

        [StringLength(100)]
        [Column("Synonym_Skill")]
        public string? SynonymName { get; set; }
    }
}