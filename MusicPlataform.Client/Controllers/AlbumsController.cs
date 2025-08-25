using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;

namespace MusicPlataform.Client.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly HttpClient httpClient;

        public AlbumsController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7106/api");
        }

        public async Task<IActionResult> Index()
        {
            var response = await httpClient.GetAsync("api/albums"); 

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var album = JsonSerializer.Deserialize<IEnumerable<AlbumClient>>(content, options);

                return View(album);
            }
            return View(new List<AlbumClient>());
        }
    }
}
