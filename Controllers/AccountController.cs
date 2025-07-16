using FinanceApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinanceApp.Web.Controllers {
    public class AccountController : Controller {
        private readonly AuthorizedApiClientFactory _apiClientFactory;

        public AccountController(AuthorizedApiClientFactory apiClientFactory) {
            _apiClientFactory = apiClientFactory;
        }

        public IActionResult Register() => View();

        #region Authentication
        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationDto dto) {
            var client = _apiClientFactory.CreateClientWithJwt();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7111/api/users/register", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ModelState.AddModelError(string.Empty, "Registration failed.");
            return View(dto);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto) {
            var client = _apiClientFactory.CreateClientWithJwt();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7111/api/users/login", content);

            if (response.IsSuccessStatusCode) {
                var tokenJson = await response.Content.ReadAsStringAsync();

                // Parse token from JSON response
                var token = JsonSerializer.Deserialize<JsonElement>(tokenJson).GetProperty("token").GetString();

                // Store JWT in cookie
                Response.Cookies.Append("jwt", token, new CookieOptions {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return RedirectToAction("Success");
            }

            ModelState.AddModelError(string.Empty, "Invalid login.");
            return View(dto);
        }

        public IActionResult Logout() {
            // Remove the JWT cookie
            Response.Cookies.Delete("jwt");

            // Redirect to home or login page
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public async Task<IActionResult> Me() {
            var client = _apiClientFactory.CreateClientWithJwt();
            var response = await client.GetAsync("https://localhost:7111/api/users/me");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var content = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<JsonElement>(content);

            ViewBag.Profile = profile;
            return View();
        }

        public IActionResult Success() {
            return View("Success");
        }
    }
}
