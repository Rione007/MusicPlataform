using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly MusicContext _context;

        public ArtistsController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/artists
        [HttpGet]
        public IActionResult GetAllArtists()
        {
            var artists = _context.Artists
                .Include(a => a.Albums)
                .Include(a => a.Tracks)
                .ToList();

            return Ok(artists);
        }

        // GET: api/artists/5
        [HttpGet("{id:int}")]
        public IActionResult GetArtistById(int id)
        {
            var artist = _context.Artists
                .Include(a => a.Albums)
                .Include(a => a.Tracks)
                .FirstOrDefault(a => a.Id == id);

            if (artist == null)
                return NotFound();

            return Ok(artist);
        }

        // POST: api/artists
        [HttpPost]
        public IActionResult CreateArtist(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();

            return Ok(artist);
        }

        // PUT: api/artists/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateArtist(int id, Artist updatedArtist)
        {
            var artist = _context.Artists.Find(id);

            if (artist == null)
                return NotFound();

            artist.Name = updatedArtist.Name;
            artist.Bio = updatedArtist.Bio;

            _context.SaveChanges();

            return Ok(artist);
        }

        // DELETE: api/artists/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteArtist(int id)
        {
            var artist = _context.Artists.Find(id);

            if (artist == null)
                return NotFound();

            _context.Artists.Remove(artist);
            _context.SaveChanges();

            return Ok();
        }
    }
}
