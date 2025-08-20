using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.GenreDtos;

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
            var genres = _context.Genres
                .Select(g => new GenreReadDto(g.Id, g.Name))
                .ToList();

            return Ok(genres);
        }

        // GET: api/genres/5
        [HttpGet("{id:int}")]
        public IActionResult GetGenreById(int id)
        {
            var genre = _context.Genres
                .Where(g => g.Id == id)
                .Select(g => new GenreReadDto(g.Id, g.Name))
                .FirstOrDefault();

            if (genre == null)
                return NotFound();

            return Ok(genre);
        }

        // POST: api/genres
        [HttpPost]
        public IActionResult CreateGenre(GenreCreateDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            _context.Genres.Add(genre);
            _context.SaveChanges();

            return Ok(new GenreReadDto(genre.Id, genre.Name));
        }

        // PUT: api/genres/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateGenre(int id, GenreCreateDto dto)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null)
                return NotFound();

            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(new GenreReadDto(genre.Id, genre.Name));
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
