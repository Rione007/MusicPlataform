using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.ArtistDtos;

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
                .Select(a => new ArtistReadDto(a.Id, a.Name, a.Bio))
                .ToList();

            return Ok(artists);
        }

        // GET: api/artists/5
        [HttpGet("{id:int}")]
        public IActionResult GetArtistById(int id)
        {
            var artist = _context.Artists
                .Where(a => a.Id == id)
                .Select(a => new ArtistReadDto(a.Id, a.Name, a.Bio))
                .FirstOrDefault();

            if (artist == null)
                return NotFound();

            return Ok(artist);
        }

        // POST: api/artists
        [HttpPost]
        public IActionResult CreateArtist(ArtistCreateDto dto)
        {
            var artist = new Artist
            {
                Name = dto.Name,
                Bio = dto.Bio
            };

            _context.Artists.Add(artist);
            _context.SaveChanges();

            return Ok(new ArtistReadDto(artist.Id, artist.Name, artist.Bio));
        }

        // PUT: api/artists/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateArtist(int id, ArtistCreateDto dto)
        {
            var artist = _context.Artists.Find(id);
            if (artist == null)
                return NotFound();

            artist.Name = dto.Name;
            artist.Bio = dto.Bio;

            _context.SaveChanges();

            return Ok(new ArtistReadDto(artist.Id, artist.Name, artist.Bio));
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
