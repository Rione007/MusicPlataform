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

            // DTO que espera el API
            var dto = new
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password // en texto plano, el API lo hashea
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("users/register", content);

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

            // DTO que espera el API
            var dto = new
            {
                Username = model.Username,
                Email = string.Empty, // no se usa en login
                Password = model.Password // en texto plano, el API lo verifica
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("users/login", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            return View(model);
        }
    }
}
