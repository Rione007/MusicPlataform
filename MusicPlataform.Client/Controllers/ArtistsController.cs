using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class ArtistsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
