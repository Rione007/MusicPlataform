using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MusicPlataform.Client.Models;

namespace MusicPlataform.Client.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Users/Register
        public IActionResult Register() => View();

        // POST: Users/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("MusicApi");

            // Construir la query string para password
            var url = $"users/register?password={model.Password}";

            // Mandar el objeto User (Username + Email)
            var content = new StringContent(
                JsonSerializer.Serialize(new { model.Username, model.Email }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ModelState.AddModelError("", "Error en el registro");
            return View(model);
        }

        // GET: Users/Login
        public IActionResult Login() => View();

        // POST: Users/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("MusicApi");

            // Mandar como query string username + password
            var url = $"users/login?username={model.Username}&password={model.Password}";
            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            return View(model);
        }
    }
}
