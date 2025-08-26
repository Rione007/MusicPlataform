using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("MusicApi");

            var dto = new
            {
                Username = model.Username,
                Email = string.Empty,
                Password = model.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("users/login", content);

            if (response.IsSuccessStatusCode)
            {
                // 👇 Aquí leemos el JSON con los datos del usuario
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserClient>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        });

                    return RedirectToAction("Index", "ArTrack");
                }
            }

            ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "ArTrack");
        }

        // GET: Perfil
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("MusicApi");
            var response = await client.GetAsync($"users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron cargar los datos.";
                return RedirectToAction("Index", "ArTrack");
            }

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserClient>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(user);
        }

        // POST: Actualizar perfil
        [HttpPost]
        public async Task<IActionResult> Profile(UserClient model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("MusicApi");

            // IMPORTANTE: el Username no se debe cambiar, así que no se envía modificado
            var dto = new
            {
                Id = model.Id,
                Username = model.Username, 
                Email = model.Email
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"users/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Perfil actualizado correctamente.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Error al actualizar el perfil.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Las contraseñas no coinciden.";
                return RedirectToAction("Profile");
            }

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("MusicApi");

            var dto = new
            {
                Id = userId,
                Password = NewPassword
            };

            var content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"users/{userId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Contraseña actualizada correctamente.";
            }
            else
            {
                TempData["Error"] = "Error al actualizar la contraseña.";
            }

            return RedirectToAction("Profile");
        }


    }
}
