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
        private readonly HttpClient httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PlaylistsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            httpClient = httpClientFactory.CreateClient("MusicApi");
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        private async Task<List<PlaylistReadDtoClient>> GetAllPlaylistsAsync(HttpClient api)
        {
            var resp = await api.GetAsync("playlists");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<PlaylistReadDtoClient>>(json, _jsonOptions) ?? new();
            return list;
        }

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

        [HttpGet]
        public async Task<IActionResult> MyPlaylistsJson()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var all = await GetAllPlaylistsAsync(api);
            var mine = all
                .Where(p => p.OwnerId == userId)
                .Select(p => new { id = p.Id, name = p.Name, isPublic = p.IsPublic })
                .OrderBy(p => p.name)
                .ToList();

            return Json(new { items = mine });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefault()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var api = _httpClientFactory.CreateClient("MusicApi");
            var all = await GetAllPlaylistsAsync(api);
            var mine = all.Where(p => p.OwnerId == userId).Select(p => p.Name).ToList();

            int n = 1;
            string name;
            do
            {
                name = $"Play List n.º {n}";
                n++;
            } while (mine.Contains(name, StringComparer.OrdinalIgnoreCase));

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

            return Json(new { id = created.Id, name = created.Name, url = Url.Action("Details", new { id = created.Id }) });
        }

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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public class PlaylistUpdateNameDto
        {
            public string Name { get; set; } = string.Empty;
        }

        // ✅ Método corregido: actualiza el nombre usando PUT /api/playlists/{id}
        [HttpPost]
        public async Task<IActionResult> UpdateName(int id, [FromBody] PlaylistUpdateNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Nombre inválido." });

            var api = _httpClientFactory.CreateClient("MusicApi");

            // Obtener la playlist actual
            var respGet = await api.GetAsync($"playlists/{id}");
            if (!respGet.IsSuccessStatusCode)
                return NotFound(new { message = "Playlist no encontrada." });

            var json = await respGet.Content.ReadAsStringAsync();
            var current = JsonSerializer.Deserialize<PlaylistReadDtoClient>(json, _jsonOptions);
            if (current == null)
                return Problem("No se pudo leer la playlist actual.");

            // Crear el payload completo requerido por PlaylistCreateDto
            var updateDto = new
            {
                Name = dto.Name,
                OwnerId = current.OwnerId,
                IsPublic = current.IsPublic
            };

            var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
            var resp = await api.PutAsync($"playlists/{id}", content);

            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { message = "No se pudo actualizar el nombre", detail = error });
            }

            return Ok(new { message = "Nombre actualizado" });
        }
    }
}
