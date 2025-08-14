using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly MusicContext _context;

        public AlbumsController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/albums
        [HttpGet]
        public IActionResult GetAllAlbums()
        {
            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .ToList();

            return Ok(albums);
        }

        // GET: api/albums/5
        [HttpGet("{id:int}")]
        public IActionResult GetAlbumById(int id)
        {
            var album = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                .FirstOrDefault(a => a.Id == id);

            if (album == null)
                return NotFound();

            return Ok(album);
        }

        // POST: api/albums
        [HttpPost]
        public IActionResult CreateAlbum(Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();

            return Ok(album);
        }

        // PUT: api/albums/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateAlbum(int id, Album updatedAlbum)
        {
            var album = _context.Albums.Find(id);

            if (album == null)
                return NotFound();

            album.Title = updatedAlbum.Title;
            album.Year = updatedAlbum.Year;
            album.ArtistId = updatedAlbum.ArtistId;
            album.CoverUrl = updatedAlbum.CoverUrl;

            _context.SaveChanges();

            return Ok(album);
        }

        // DELETE: api/albums/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteAlbum(int id)
        {
            var album = _context.Albums.Find(id);

            if (album == null)
                return NotFound();

            _context.Albums.Remove(album);
            _context.SaveChanges();

            return Ok();
        }
    }
}
