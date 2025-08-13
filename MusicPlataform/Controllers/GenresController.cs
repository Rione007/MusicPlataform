using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly MusicContext _context;

        public GenresController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/genres
        [HttpGet]
        public IActionResult GetAllGenres()
        {
            var genres = _context.Genres.ToList();
            return Ok(genres);
        }

        // GET: api/genres/5
        [HttpGet("{id:int}")]
        public IActionResult GetGenreById(int id)
        {
            var genre = _context.Genres
                .Include(g => g.Tracks)
                .FirstOrDefault(g => g.Id == id);

            if (genre == null)
                return NotFound();

            return Ok(genre);
        }

        // POST: api/genres
        [HttpPost]
        public IActionResult CreateGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        // PUT: api/genres/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateGenre(int id, Genre updatedGenre)
        {
            var genre = _context.Genres.Find(id);

            if (genre == null)
                return NotFound();

            genre.Name = updatedGenre.Name;
            _context.SaveChanges();

            return Ok(genre);
        }

        // DELETE: api/genres/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteGenre(int id)
        {
            var genre = _context.Genres.Find(id);

            if (genre == null)
                return NotFound();

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return Ok();
        }
    }
}
