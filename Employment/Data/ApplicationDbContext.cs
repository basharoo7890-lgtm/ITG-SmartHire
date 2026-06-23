using Microsoft.EntityFrameworkCore;
using Employment.Models;

namespace Employment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<SkillSynonym> SkillSynonyms { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ????? ??????? (Table Mapping)
            modelBuilder.Entity<Application>().ToTable("Application");
            modelBuilder.Entity<Job>().ToTable("Jobs");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<AIAnalysis>().ToTable("Ai_Analysis");
            modelBuilder.Entity<JobSkill>().ToTable("Job_Skill");
            modelBuilder.Entity<SkillSynonym>().ToTable("Skill_Synonyms");
            modelBuilder.Entity<SystemSetting>().ToTable("System_Setting");

            // ? ?????? ??????? ????? ??????? (Precision and Scale)
            // ?????? HasPrecision(18, 2) ????? ??? ??????? (Currency Precision)

            modelBuilder.Entity<Application>()
                .Property(a => a.ExpectedSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMin)
                .HasPrecision(18, 2);

            // ???????? (Relationships)
            modelBuilder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}