using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        private readonly MusicContext _context;

        public TracksController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/tracks
        [HttpGet]
        public IActionResult GetAllTracks()
        {
            var tracks = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .Include(t => t.Genre)
                .ToList();
            return Ok(tracks);
        }

        // GET: api/tracks/5
        [HttpGet("{id:int}")]
        public IActionResult GetTrackById(int id)
        {
            var track = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .Include(t => t.Genre)
                .FirstOrDefault(t => t.Id == id);

            if (track == null)
                return NotFound();

            return Ok(track);
        }

        // POST: api/tracks
        [HttpPost]
        public IActionResult CreateTrack(Track track)
        {
            _context.Tracks.Add(track);
            _context.SaveChanges();
            return Ok(track);
        }

        // PUT: api/tracks/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateTrack(int id, Track updatedTrack)
        {
            var track = _context.Tracks.Find(id);

            if (track == null)
                return NotFound();

            track.Title = updatedTrack.Title;
            track.DurationSeconds = updatedTrack.DurationSeconds;
            track.ArtistId = updatedTrack.ArtistId;
            track.AlbumId = updatedTrack.AlbumId;
            track.GenreId = updatedTrack.GenreId;
            track.AudioUrl = updatedTrack.AudioUrl;

            _context.SaveChanges();

            return Ok(track);
        }

        // DELETE: api/tracks/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteTrack(int id)
        {
            var track = _context.Tracks.Find(id);

            if (track == null)
                return NotFound();

            _context.Tracks.Remove(track);
            _context.SaveChanges();

            return Ok();
        }
    }
}
