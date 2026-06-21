using Employment.Data;
using Employment.Interfaces;
using Employment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 1. Load the variables from your local .env file into the system environment
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Tell .NET to inject environment variables into builder.Configuration
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();