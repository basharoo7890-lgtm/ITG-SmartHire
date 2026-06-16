using Employment.Data;
using Employment.Interfaces;
using Employment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// تحميل المتغيرات من ملف .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// دمج متغيرات البيئة في التكوين
builder.Configuration.AddEnvironmentVariables();

// إعداد قاعدة البيانات مع تحديد Scoped لضمان الأمان في التعامل مع البيانات
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

// إعداد المصادقة (Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

// تسجيل الخدمات (Dependency Injection)
// ملاحظة: تحويل الخدمات التي تتعامل مع البيانات إلى Scoped بدلاً من Singleton لتجنب الانهيار (Crash)
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<CVParserService>();
builder.Services.AddScoped<LanguageDetectorService>();
builder.Services.AddScoped<CVCompletenessService>();
builder.Services.AddScoped<JobDescriptionService>();
builder.Services.AddScoped<AIAnalysisService>();
builder.Services.AddScoped<SkillsGapService>();

// تسجيل HttpClient لخدمة الذكاء الاصطناعي مع إعدادات الوقت (Timeout) لمنع تعليق النظام
builder.Services.AddHttpClient<GeminiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// معالجة الأخطاء في بيئة الإنتاج
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// تشغيل التطبيق مع مراقبة الأخطاء الكارثية
try
{
    app.Run();
}
catch (Exception ex)
{
    // سجل الخطأ في وحدة التحكم لتسهيل التصحيح
    Console.WriteLine($"Fatal Error: {ex.Message}");
    throw;
}