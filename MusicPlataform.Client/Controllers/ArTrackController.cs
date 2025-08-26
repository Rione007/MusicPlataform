using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;

namespace MusicPlataform.Client.Controllers
{
    public class ArTrackController : Controller
    {
        private readonly HttpClient httpClient;

        public ArTrackController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7106/api");
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ArTrackViewModel();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var artistResponse = await httpClient.GetAsync("api/artists");
            if (artistResponse.IsSuccessStatusCode)
            {
                var artistContent = await artistResponse.Content.ReadAsStringAsync();
                var artistas = JsonSerializer.Deserialize<IEnumerable<ArtistClient>>(artistContent, options);
                viewModel.Artistas = artistas;
            }
            else
            {
                viewModel.Artistas = new List<ArtistClient>();
            }

            var trackResponse = await httpClient.GetAsync("api/tracks");
            if (trackResponse.IsSuccessStatusCode)
            {
                var trackContent = await trackResponse.Content.ReadAsStringAsync();
                var tracks = JsonSerializer.Deserialize<IEnumerable<TrackClient>>(trackContent, options);
                viewModel.Tracks = tracks;
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

            var artistResponse = await httpClient.GetAsync($"api/artists/{id}");
            if (!artistResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var artistContent = await artistResponse.Content.ReadAsStringAsync();
            var artista = JsonSerializer.Deserialize<ArtistClient>(artistContent, options);

            var trackResponse = await httpClient.GetAsync("api/tracks");
            var tracks = new List<TrackClient>();

            if (trackResponse.IsSuccessStatusCode)
            {
                var trackContent = await trackResponse.Content.ReadAsStringAsync();
                var allTracks = JsonSerializer.Deserialize<List<TrackClient>>(trackContent, options);

                var artistName = artista.Name;

                tracks = allTracks.Where(t => t.Artist == artistName).ToList();

            }

            var viewModel = new ArtistDetailViewModel
            {
                Artista = artista,
                Tracks = tracks
            };

            return View(viewModel);
        }


    }
}
