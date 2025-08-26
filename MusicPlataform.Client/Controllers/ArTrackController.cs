using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;
using System;
using System.Text;

namespace MusicPlataform.Client.Controllers
{
    public class ArTrackController : Controller
    {
        private readonly HttpClient httpClient;

        public ArTrackController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7106/api/");
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ArTrackViewModel();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var artistResponse = await httpClient.GetAsync("artists");
            if (artistResponse.IsSuccessStatusCode)
            {
                var artistContent = await artistResponse.Content.ReadAsStringAsync();
                var artistas = JsonSerializer.Deserialize<IEnumerable<ArtistClient>>(artistContent, options);
                viewModel.Artistas = artistas ?? new List<ArtistClient>();
            }
            else
            {
                viewModel.Artistas = new List<ArtistClient>();
            }

            var trackResponse = await httpClient.GetAsync("tracks");
            if (trackResponse.IsSuccessStatusCode)
            {
                var trackContent = await trackResponse.Content.ReadAsStringAsync();
                var tracks = JsonSerializer.Deserialize<IEnumerable<TrackClient>>(trackContent, options);
                viewModel.Tracks = tracks ?? new List<TrackClient>();
            }
            else
            {
                viewModel.Tracks = new List<TrackClient>();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ArtistaDetalle(int id)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var artistResponse = await httpClient.GetAsync($"artists/{id}");
            if (!artistResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var artistContent = await artistResponse.Content.ReadAsStringAsync();
            var artista = JsonSerializer.Deserialize<ArtistClient>(artistContent, options);

            var trackResponse = await httpClient.GetAsync("tracks");
            var tracks = new List<TrackClient>();

            if (trackResponse.IsSuccessStatusCode)
            {
                var trackContent = await trackResponse.Content.ReadAsStringAsync();
                var allTracks = JsonSerializer.Deserialize<List<TrackClient>>(trackContent, options);

                if (artista != null)
                {
                    var artistName = artista.Name;
                    tracks = allTracks?.Where(t => t.Artist == artistName).ToList() ?? new List<TrackClient>();
                }
            }

            var viewModel = new ArtistDetailViewModel
            {
                Artista = artista,
                Tracks = tracks
            };

            return View(viewModel);
        }

        // POST: ArTrack/AddToLibrary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToLibrary(int trackId, int artistId)
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Users");
            }

            var dto = new { TrackId = trackId };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var encodedUser = Uri.EscapeDataString(username);
            var response = await httpClient.PostAsync($"playlists/user/{encodedUser}/library/addtrack", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["LibraryError"] = await response.Content.ReadAsStringAsync();
            }
            else
            {
                TempData["LibrarySuccess"] = "Canción agregada a tu biblioteca";
            }

            return RedirectToAction("ArtistaDetalle", new { id = artistId });
        }

        // GET: ArTrack/MiBiblioteca
        public async Task<IActionResult> MiBiblioteca()
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Users");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var encodedUser = Uri.EscapeDataString(username);
            var response = await httpClient.GetAsync($"playlists/user/{encodedUser}/library");

            if (!response.IsSuccessStatusCode)
            {
                TempData["LibraryError"] = "No se pudo obtener tu biblioteca";
                return View(new List<TrackClient>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var tracks = JsonSerializer.Deserialize<List<TrackClient>>(json, options) ?? new List<TrackClient>();

            return View(tracks);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromLibrary(int trackId)
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Users");
            }

            var encodedUser = Uri.EscapeDataString(username);
            var response = await httpClient.DeleteAsync($"playlists/user/{encodedUser}/library/removetrack/{trackId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["LibraryError"] = await response.Content.ReadAsStringAsync();
            }
            else
            {
                TempData["LibrarySuccess"] = "Canción eliminada de tu biblioteca";
            }

            return RedirectToAction("MiBiblioteca");
        }



    }
}
