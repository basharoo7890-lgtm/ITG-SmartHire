using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("Ai_Analysis")]
    public class AIAnalysis
    {
        [Key]
        [Column("Ai_Id")]
        public int AIAnalysisId { get; set; }

        [Required]
        [Column("App_Id")]
        public int ApplicationId { get; set; }

        [Column("Matching_Score", TypeName = "decimal(5,2)")]
        public decimal? MatchingScore { get; set; }

        public string? Summary { get; set; }

        public string? Strengths { get; set; }

        public string? Weaknesses { get; set; }

        public DateTime? AnalysisDate { get; set; }

        public string? ParsedSkills { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ExperienceScore { get; set; }

        public string? InterviewQuestions { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SkillsScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SalaryScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? EducationScore { get; set; }

        [ForeignKey("ApplicationId")]
        public Application? Application { get; set; }

    }
}