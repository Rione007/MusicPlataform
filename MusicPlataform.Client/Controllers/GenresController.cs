using Microsoft.AspNetCore.Mvc;

namespace MusicPlataform.Client.Controllers
{
    public class GenresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
