using Employment.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Employment.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedUserAsync(context, config, logger, "Admin");
            await SeedUserAsync(context, config, logger, "HR");
        }

        private static async Task SeedUserAsync(
            ApplicationDbContext context,
            IConfiguration config,
            ILogger logger,
            string role)
        {
            // Program.cs calls builder.Configuration.AddEnvironmentVariables()
            // so ADMIN_EMAIL in .env becomes config["ADMIN_EMAIL"]
            var prefix   = role.ToUpper();
            var email    = config[$"{prefix}_EMAIL"];
            var password = config[$"{prefix}_PASSWORD"];
            var name     = config[$"{prefix}_NAME"] ?? role;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning(
                    "Seeder: {Role} credentials not set in environment variables " +
                    "({Prefix}_EMAIL / {Prefix}_PASSWORD). Skipping.",
                    role, prefix, prefix);
                return;
            }

            // Check if user already exists
            var exists = await context.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                // Update role in case user registered as Applicant before
                var existing = await context.Users.FirstAsync(u => u.Email == email);
                if (existing.Role != role)
                {
                    existing.Role = role;
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeder: Updated {Email} role to {Role}.", email, role);
                }
                return;
            }

            // Create new user
            var user = new User
            {
                FullName     = name,
                Email        = email,
                PasswordHash = HashPassword(password),
                Role         = role,
                CreatedAt    = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeder: Created {Role} user → {Email}", role, email);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
