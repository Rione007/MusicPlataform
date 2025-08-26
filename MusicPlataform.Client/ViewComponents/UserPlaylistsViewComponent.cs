using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using MusicPlataform.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlataform.Client.ViewComponents
{
    public class UserPlaylistsViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public UserPlaylistsViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7106/api/");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var username = HttpContext.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                // return an empty view indicating the user is not logged in
                return View(new List<PlaylistClient>());
            }

            var encoded = Uri.EscapeDataString(username);
            var response = await _httpClient.GetAsync($"playlists/user/{encoded}");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<PlaylistClient>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var playlists = JsonSerializer.Deserialize<List<PlaylistClient>>(content, options) ?? new List<PlaylistClient>();
            return View(playlists);
        }
    }
}
