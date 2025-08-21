using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class ArtistsController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
