using Microsoft.AspNetCore.Mvc;
using Assignment.Models;
using Assignment.Services;
using System.Threading.Tasks;
using Assignment.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Assignment.Framework;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        ILogger<HomeController> _logger;
        HttpClient _httpClient;
        IAccountService _context;
        IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, IAccountService context, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
        }

        // Index action to display home page
        public IActionResult Index()
        {
            return View();
        }

        // Action to fetch and display exchange rate
        public async Task<IActionResult> ExchangeRate()
        {
            var apiUrl = "https://www.nrb.org.np/api/forex/v1/rates?page=1&per_page=5&from=2024-06-12&to=2024-06-12";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize into a dynamic object to access nested properties
            dynamic jsonResponse = JsonConvert.DeserializeObject(response);

            // Extract rates list from the nested JSON structure
            var rates = jsonResponse?.data?.payload[0]?.rates;

            var exchangeRateList = new List<ExchangeRateModel>();

            foreach (var rate in rates)
            {
                decimal buyRate = 0m;
                decimal sellRate = 0m;

                // Try parsing the buy and sell values
                if (!decimal.TryParse(rate.buy.ToString(), out buyRate))
                {
                    buyRate = 0m; // Default value if parsing fails
                }

                if (!decimal.TryParse(rate.sell.ToString(), out sellRate))
                {
                    sellRate = 0m; // Default value if parsing fails
                }

                var exchangeRateModel = new ExchangeRateModel
                {
                    Currency = rate.currency.name,
                    Buy = buyRate,
                    Sell = sellRate
                };

                exchangeRateList.Add(exchangeRateModel);
            }

            return View(exchangeRateList);


        }
        // Action to handle money transfer from Malaysia to Nepal
        [HttpGet]
        public async Task<IActionResult> TransferMoney()
        {
            var apiUrl = "https://www.nrb.org.np/api/forex/v1/rates?page=1&per_page=5&from=2024-06-12&to=2024-06-12";
            var response = await _httpClient.GetStringAsync(apiUrl);
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);

            var rates = jsonResponse?.data?.payload[0]?.rates;
            var currencyList = new List<SelectListItem>();

            // Create a dictionary to pass currency codes with their sell rates
            var currencyRates = new Dictionary<string, decimal>();

            foreach (var rate in rates)
            {
                decimal sellRate = 0m;
                var currencyCode = rate.currency.name.ToString();

                if (!decimal.TryParse(rate.sell.ToString(), out sellRate))
                {
                    sellRate = 0m; // Default value if parsing fails
                }
                // Add currency to list and dictionary for dropdown and JavaScript access
                currencyList.Add(new SelectListItem
                {
                    Value = currencyCode,
                    Text = rate.currency.name.ToString()
                });
                currencyRates[currencyCode] = sellRate;
            }

            ViewData["Currencies"] = currencyList;
            ViewData["CurrencyRates"] = JsonConvert.SerializeObject(currencyRates); // Pass rates as JSON

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferMoney(MoneyTransferModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _context.transferMoney(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Money transfer successfully!";
                    return RedirectToAction("TransferMoney");
                }
                else
                {
                    ModelState.AddModelError("", "Error occurred during transfer.");
                }
            }

            return View(model);
        }
        public async Task<IActionResult> TransactionReport(string fromDate, string toDate)
        {
            // Extract the JWT token from the Authorization header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing or invalid");
            }

            try
            {
                // Validate the token (you can use your JWT service here)
                var claimsPrincipal = ValidateToken(token);

                // If the token is valid, extract user information (optional)
                var userId = claimsPrincipal?.FindFirst("Id")?.Value;

                // Use fromDate and toDate, set defaults if not provided
                fromDate = string.IsNullOrWhiteSpace(fromDate) ? DateTime.Now.ToString("yyyy-MM-dd") : fromDate;
                toDate = string.IsNullOrWhiteSpace(toDate) ? DateTime.Now.ToString("yyyy-MM-dd") : toDate;

                var transactions = await _context.GetData(fromDate, toDate);

                // Return JSON data if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(transactions);
                }

                return View(transactions);
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid token: {ex.Message}");
            }
        }
        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                return null; // Invalid token
            }
        }

    }
}
