using AspNetCore.ReCaptcha;
using Assignment.Data;
using Assignment.Framework;
using Assignment.Models;
using Assignment.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.SymbolStore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Add a global authorization filter
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

// Register HttpClient
builder.Services.AddHttpClient();

// Register HttpContextAccessor (required for IHttpContextAccessor to work)
builder.Services.AddHttpContextAccessor();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));

// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IAccountService and AccountService
builder.Services.AddScoped<IAccountService, AccountService>();
//Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// Add Authentication Services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to login
        options.LogoutPath = "/Account/Logout"; // Redirect to logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session expiration
        options.SlidingExpiration = true; // Extend expiration on activity
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Redirect to the LoginPath without appending the ReturnUrl
                context.Response.Redirect(options.LoginPath);
                return Task.CompletedTask;
            }
        };
    });


// Add Session Middleware
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Enable session middleware
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
