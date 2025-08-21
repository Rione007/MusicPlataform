using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class TracksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
