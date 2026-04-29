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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<Application>().ToTable("Application");
            modelBuilder.Entity<Job>().ToTable("Jobs");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}