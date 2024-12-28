using Microsoft.AspNetCore.Mvc;
using Assignment.Models;
using System.Configuration;
using System.Threading.Tasks;
using Assignment.Framework;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Assignment.Controllers
{
    public class AccountController : Controller
    {
         ILogger<HomeController> _logger;
         IAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<HomeController> logger, IAccountService accountService, IConfiguration configuration)
        {
            _logger = logger;
            _accountService = accountService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Handle the login form submission
        [HttpPost]
        [ValidateReCaptcha]
        public async Task<IActionResult> Login(LoginCredModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _accountService.AuthenticateUser(model);
                if (user != null)
                {
                    //Generate token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name,user.Username),
                            new Claim(ClaimTypes.Email,user.Email),
                            new Claim("Id",user.Id.ToString()),
                            new Claim("Status",user.Status)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = _configuration["Jwt:Issuer"],
                        Audience = _configuration["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    HttpContext.Response.Headers.Add("Authorization", $"Bearer {jwtToken}");
                    return RedirectToAction("ExChangeRate", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
        }

        // Display the registration page

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateReCaptcha]
        public async Task<IActionResult> SaveRegister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterUserAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Error occurred during registration.");
                }
            }
            return View("Register", model);
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

    }
}
