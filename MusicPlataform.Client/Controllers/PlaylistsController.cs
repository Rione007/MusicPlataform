using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;
using System.Security.Claims;

namespace MusicPlataform.Client.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PlaylistsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Helper: get user id from claims
        private int? GetUserId()
        {
            var idStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // Helper: get all playlists from API
        private async Task<List<PlaylistReadDtoClient>> GetAllPlaylistsAsync(HttpClient api)
        {
            var resp = await api.GetAsync("playlists");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<PlaylistReadDtoClient>>(json, _jsonOptions) ?? new();
            return list;
        }

        // Helper: ensure the user's 'Mi Biblioteca' playlist exists
        private async Task<PlaylistReadDtoClient> EnsureLibraryPlaylistAsync(HttpClient api, int userId)
        {
            var all = await GetAllPlaylistsAsync(api);
            var library = all.FirstOrDefault(p => p.OwnerId == userId && p.Name == "Mi Biblioteca");
            if (library != null) return library;

            // Create it (private)
            var payload = new
            {
                Name = "Mi Biblioteca",
                OwnerId = userId,
                IsPublic = false
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var create = await api.PostAsync("playlists", content);
            create.EnsureSuccessStatusCode();
            var json = await create.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<PlaylistReadDtoClient>(json, _jsonOptions);
            if (created == null) throw new InvalidOperationException("No se pudo crear la playlist 'Mi Biblioteca'.");
            return created;
        }

        // Helper: get all tracks and filter by ids
        private async Task<List<TrackClient>> GetTracksByIdsAsync(HttpClient api, IEnumerable<int> ids)
        {
            var set = new HashSet<int>(ids);
            if (set.Count == 0) return new List<TrackClient>();
            var resp = await api.GetAsync("tracks");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var all = JsonSerializer.Deserialize<List<TrackClient>>(json, _jsonOptions) ?? new();
            return all.Where(t => set.Contains(t.Id)).ToList();
        }

        // GET: JSON for sidebar library
        [HttpGet]
        public async Task<IActionResult> MyLibraryJson()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");

            // Ensure (but don't create) library exists; if not, return empty
            var all = await GetAllPlaylistsAsync(api);
            var library = all.FirstOrDefault(p => p.OwnerId == userId && p.Name == "Mi Biblioteca");
            if (library == null) return Json(new { items = Array.Empty<object>() });

            var tracks = await GetTracksByIdsAsync(api, library.TrackIds);
            var items = tracks.Select(t => new { id = t.Id, title = t.Title, artist = t.Artist, audioUrl = t.AudioUrl });
            return Json(new { items });
        }

        // POST: Add a track to 'Mi Biblioteca'
        [HttpPost]
        public async Task<IActionResult> AddToLibrary(int trackId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var library = await EnsureLibraryPlaylistAsync(api, userId.Value);

            // Avoid duplicates
            if (library.TrackIds.Contains(trackId))
            {
                return Conflict(new { message = "La canción ya está en tu biblioteca." });
            }

            var dto = new PlaylistAddTrackDto { TrackId = trackId, Order = null };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var resp = await api.PostAsync($"playlists/{library.Id}/tracks", content);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "Error al agregar la canción.", detail = error });
            }

            return Ok(new { message = "Agregado a tu biblioteca.", playlistId = library.Id, trackId });
        }

        // DELETE: Remove a track from 'Mi Biblioteca'
        [HttpPost]
        public async Task<IActionResult> RemoveFromLibrary(int trackId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var all = await GetAllPlaylistsAsync(api);
            var library = all.FirstOrDefault(p => p.OwnerId == userId && p.Name == "Mi Biblioteca");
            if (library == null) return NotFound(new { message = "No tienes biblioteca creada." });

            var resp = await api.DeleteAsync($"playlists/{library.Id}/tracks/{trackId}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "Error al quitar la canción.", detail = error });
            }

            return Ok(new { message = "Quitado de tu biblioteca." });
        }


        // ====== NUEVO: Mis listas (además de "Mi Biblioteca") ======

        // GET: JSON con las playlists del usuario (excluye "Mi Biblioteca")
        [HttpGet]
        public async Task<IActionResult> MyPlaylistsJson()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var all = await GetAllPlaylistsAsync(api);
            var mine = all
                .Where(p => p.OwnerId == userId && p.Name != "Mi Biblioteca")
                .Select(p => new { id = p.Id, name = p.Name, isPublic = p.IsPublic })
                .OrderBy(p => p.name)
                .ToList();

            return Json(new { items = mine });
        }

        // POST: Crea una lista vacía con un nombre por defecto (Mi lista n.º X)
        [HttpPost]
        public async Task<IActionResult> CreateDefault()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var all = await GetAllPlaylistsAsync(api);
            var mine = all.Where(p => p.OwnerId == userId).Select(p => p.Name).ToList();

            // Buscar el siguiente número disponible
            int n = 1;
            string name;
            do
            {
                name = $"Mi lista n.º {n}";
                n++;
            } while (mine.Contains(name, StringComparer.OrdinalIgnoreCase));

            // Crear (privada por defecto)
            var payload = new { Name = name, OwnerId = userId.Value, IsPublic = false };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await api.PostAsync("playlists", content);
            if (!resp.IsSuccessStatusCode)
            {
                var detail = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "No se pudo crear la lista.", detail });
            }

            var json = await resp.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<PlaylistReadDtoClient>(json, _jsonOptions);
            if (created == null) return Problem("Respuesta inválida del servidor.");

            // Devolver datos y URL para redirigir
            return Json(new { id = created.Id, name = created.Name, url = Url.Action("Details", new { id = created.Id }) });
        }

        // GET: Vista de una playlist
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var api = _httpClientFactory.CreateClient("MusicApi");
            var resp = await api.GetAsync($"playlists/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var playlist = JsonSerializer.Deserialize<PlaylistReadDtoClient>(json, _jsonOptions);
            if (playlist == null) return NotFound();

            return View(playlist);
        }

        // GET: JSON con los tracks de una playlist
        [HttpGet]
        public async Task<IActionResult> PlaylistJson(int id)
        {
            var api = _httpClientFactory.CreateClient("MusicApi");
            var resp = await api.GetAsync($"playlists/{id}");
            if (!resp.IsSuccessStatusCode) return NotFound();

            var json = await resp.Content.ReadAsStringAsync();
            var playlist = JsonSerializer.Deserialize<PlaylistReadDtoClient>(json, _jsonOptions);
            if (playlist == null) return NotFound();

            var tracks = await GetTracksByIdsAsync(api, playlist.TrackIds);
            var items = tracks.Select(t => new { id = t.Id, title = t.Title, artist = t.Artist, audioUrl = t.AudioUrl });
            return Json(new { items });
        }

        // POST: Agregar track a una playlist
        [HttpPost]
        public async Task<IActionResult> AddTrack(int id, int trackId)
        {
            var api = _httpClientFactory.CreateClient("MusicApi");
            var dto = new PlaylistAddTrackDto { TrackId = trackId, Order = null };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var resp = await api.PostAsync($"playlists/{id}/tracks", content);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "No se pudo agregar a la lista.", detail = error });
            }
            return Ok(new { message = "Agregado", id, trackId });
        }

        // POST: Quitar track de una playlist
        [HttpPost]
        public async Task<IActionResult> RemoveTrack(int id, int trackId)
        {
            var api = _httpClientFactory.CreateClient("MusicApi");
            var resp = await api.DeleteAsync($"playlists/{id}/tracks/{trackId}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "No se pudo quitar de la lista.", detail = error });
            }
            return Ok(new { message = "Quitado", id, trackId });
        }
        // Optional: simple Index view that shows library page (not required for sidebar)
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // You can create a Razor view if needed
        }
    }
}
