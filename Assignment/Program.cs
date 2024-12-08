using AspNetCore.ReCaptcha;
using Assignment.Data;
using Assignment.Framework;
using Assignment.Models;
using Assignment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpClient
builder.Services.AddHttpClient();

// Register HttpContextAccessor (this is required for IHttpContextAccessor to work)
builder.Services.AddHttpContextAccessor();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IAccountService and AccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// Add Authentication Services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Set your login path
        options.LogoutPath = "/Account/Logout"; // Set your logout path
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set the expiration time
        options.SlidingExpiration = true; // Optional: Keeps extending the expiration time on activity
    });
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession(); // Enable session middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // This should come before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
