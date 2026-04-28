using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Filters;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة خدمة الـ Controllers والـ Views
builder.Services.AddControllersWithViews();

// إضافة Response Caching لتسريع الاستجابات المتكررة
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

// 2. تسجيل IHttpContextAccessor (هام جداً لعمل الهيدر والسلة)
builder.Services.AddHttpContextAccessor();

// 3. إعداد قاعدة البيانات (يفضل ضبط PHONESTORE_DB_CONNECTION كمتغير بيئة في الإنتاج)
var connectionString =
    Environment.GetEnvironmentVariable("PHONESTORE_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured. Set PHONESTORE_DB_CONNECTION env var or ConnectionStrings:DefaultConnection in appsettings.Development.json.");
}

builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(connectionString));

// 4. إضافة خدمات الجلسات (Session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 5. تسجيل فلتر الحماية
builder.Services.AddScoped<AdminAuthFilter>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.Context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/webfonts/", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=2592000,immutable";
        }
    }
});

app.UseRouting();
app.UseResponseCaching();

// 6. تفعيل الجلسات
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();