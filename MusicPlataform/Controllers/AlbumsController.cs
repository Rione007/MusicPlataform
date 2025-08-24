using Microsoft.AspNetCore.Mvc;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.AlbumDtos;

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
                .Select(a => new AlbumReadDto(a.Id, a.Title, a.Year, a.ArtistId, a.CoverUrl))
                .ToList();

            return Ok(albums);
        }

        // GET: api/albums/5
        [HttpGet("{id:int}")]
        public IActionResult GetAlbumById(int id)
        {
            var album = _context.Albums
                .Where(a => a.Id == id)
                .Select(a => new AlbumReadDto(a.Id, a.Title, a.Year, a.ArtistId, a.CoverUrl))
                .FirstOrDefault();

            if (album == null)
                return NotFound();

            return Ok(album);
        }

        // POST: api/albums
        [HttpPost]
        public IActionResult CreateAlbum(AlbumCreateDto dto)
        {
            var album = new Album
            {
                Title = dto.Title,
                Year = dto.Year,
                ArtistId = dto.ArtistId,
                CoverUrl = dto.CoverUrl
            };

            _context.Albums.Add(album);
            _context.SaveChanges();

            return Ok(new AlbumReadDto(album.Id, album.Title, album.Year, album.ArtistId, album.CoverUrl));
        }

        // PUT: api/albums/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateAlbum(int id, AlbumCreateDto dto)
        {
            var album = _context.Albums.Find(id);
            if (album == null)
                return NotFound();

            album.Title = dto.Title;
            album.Year = dto.Year;
            album.ArtistId = dto.ArtistId;
            album.CoverUrl = dto.CoverUrl;

            _context.SaveChanges();

            return Ok(new AlbumReadDto(album.Id, album.Title, album.Year, album.ArtistId, album.CoverUrl));
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
