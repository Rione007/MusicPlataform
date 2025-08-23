using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class AlbumsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
