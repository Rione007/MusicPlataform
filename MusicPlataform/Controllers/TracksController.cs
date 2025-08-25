using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.TrackDtos;

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
                .Include(t => t.Genre)
                .Select(t => new TrackReadDto(
                    t.Id,
                    t.Title,
                    t.DurationSeconds,
                    t.Artist != null ? t.Artist.Name : "Unknown Artist",
                    t.Genre != null ? t.Genre.Name : null,
                    t.AudioUrl
                ))
                .ToList();

            return Ok(tracks);
        }

        // GET: api/tracks/5
        [HttpGet("{id:int}")]
        public IActionResult GetTrackById(int id)
        {
            var track = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Genre)
                .Where(t => t.Id == id)
                .Select(t => new TrackReadDto(
                    t.Id,
                    t.Title,
                    t.DurationSeconds,
                    t.Artist != null ? t.Artist.Name : "Unknown Artist",
                    t.Genre != null ? t.Genre.Name : null,
                    t.AudioUrl
                ))
                .FirstOrDefault();

            if (track == null)
                return NotFound();

            return Ok(track);
        }

        // POST: api/tracks
        [HttpPost]
        public IActionResult CreateTrack(TrackCreateDto dto)
        {
            var track = new Track
            {
                Title = dto.Title,
                DurationSeconds = dto.DurationSeconds,
                ArtistId = dto.ArtistId,
                GenreId = dto.GenreId,
                AudioUrl = dto.AudioUrl
            };

            _context.Tracks.Add(track);
            _context.SaveChanges();

            // Devolver el objeto creado en formato DTO
            var trackRead = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Genre)
                .Where(t => t.Id == track.Id)
                .Select(t => new TrackReadDto(
                    t.Id,
                    t.Title,
                    t.DurationSeconds,
                    t.Artist != null ? t.Artist.Name : "Unknown Artist",
                    t.Genre != null ? t.Genre.Name : null,
                    t.AudioUrl
                ))
                .FirstOrDefault();

            return Ok(trackRead);
        }

        // PUT: api/tracks/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateTrack(int id, TrackCreateDto dto)
        {
            var track = _context.Tracks.Find(id);
            if (track == null)
                return NotFound();

            track.Title = dto.Title;
            track.DurationSeconds = dto.DurationSeconds;
            track.ArtistId = dto.ArtistId;
            track.GenreId = dto.GenreId;
            track.AudioUrl = dto.AudioUrl;

            _context.SaveChanges();

            var updatedTrack = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Genre)
                .Where(t => t.Id == track.Id)
                .Select(t => new TrackReadDto(
                    t.Id,
                    t.Title,
                    t.DurationSeconds,
                    t.Artist != null ? t.Artist.Name : "Unknown Artist",
                    t.Genre != null ? t.Genre.Name : null,
                    t.AudioUrl
                ))
                .FirstOrDefault();

            return Ok(updatedTrack);
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
