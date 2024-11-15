using Microsoft.AspNetCore.Mvc;
using Assignment.Models;
using Assignment.Services;
using System.Threading.Tasks;
using Assignment.Framework;

namespace Assignment.Controllers
{
    public class AccountController : Controller
    {
         ILogger<HomeController> _logger;
         IAccountService _accountService;

        public AccountController(ILogger<HomeController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        // Display the login page
        public IActionResult Login()
        {
            return View();
        }

        // Handle the login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginCredModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _accountService.AuthenticateUser(model);
                if (user != null)
                {
                    // Store user information in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetInt32("id", user.Id);
                    HttpContext.Session.SetString("MobileNumber", user.MobileNumber);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Status", user.Status);
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
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
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
