using Employment.Data;
using Employment.Interfaces;
using Employment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 1. Load .env file only in Development (not in production Docker containers)
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    DotNetEnv.Env.Load();
}

// 2. Inject environment variables into configuration
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// 3. Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// 6. Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // Only redirect in production (reverse proxy handles it in Docker)
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();