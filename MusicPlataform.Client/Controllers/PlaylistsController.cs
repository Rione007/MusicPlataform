using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class PlaylistsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
