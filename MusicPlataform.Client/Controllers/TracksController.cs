using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Client.Models;

namespace MusicPlataform.Client.Controllers
{
    public class TracksController : Controller
    {
        private readonly HttpClient httpClient;

        public TracksController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient("MusicApi");
        }

        public async Task<IActionResult> Index()
        {         
            return View();
        }
    }
}
