using Employment.Data;
using Employment.Interfaces;
using Employment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 1. Load .env file only in Development (not in production Docker containers)
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    // .env lives in the repo root (parent of the project folder)
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(envPath))
        envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");

    DotNetEnv.Env.Load(envPath);
}

// 2. Inject environment variables into configuration
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// 3. Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// 4. Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// 5. Services
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddSingleton<CVParserService>();
builder.Services.AddSingleton<LanguageDetectorService>();
builder.Services.AddSingleton<CVCompletenessService>();
builder.Services.AddScoped<JobDescriptionService>();
builder.Services.AddScoped<AIAnalysisService>();
builder.Services.AddScoped<SkillsGapService>();

var app = builder.Build();

// Auto-apply pending migrations (creates the database if it doesn't exist)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Seed Admin and HR users from environment variables
var seedLogger = app.Services.GetRequiredService<ILogger<Program>>();
await DbSeeder.SeedAsync(app.Services, app.Configuration, seedLogger);

// 6. Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Trust Reverse Proxy headers (Koyeb / Nginx / AWS ALB)
// هذا يجعل التطبيق يتعرف على IP المستخدم الحقيقي وبروتوكول HTTPS
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health Check endpoint — used by Koyeb, AWS ALB, Docker HEALTHCHECK
// يُستخدم للتحقق من أن التطبيق يعمل بشكل صحيح
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();