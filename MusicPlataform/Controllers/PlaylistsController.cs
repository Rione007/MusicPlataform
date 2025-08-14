using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly MusicContext _context;

        public PlaylistsController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/playlists
        [HttpGet]
        public IActionResult GetAllPlaylists()
        {
            var playlists = _context.Playlists
                .Include(p => p.Tracks)
                .ToList();

            return Ok(playlists);
        }

        // GET: api/playlists/5
        [HttpGet("{id:int}")]
        public IActionResult GetPlaylistById(int id)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            return Ok(playlist);
        }

        // POST: api/playlists
        [HttpPost]
        public IActionResult CreatePlaylist(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return Ok(playlist);
        }

        // PUT: api/playlists/5
        [HttpPut("{id:int}")]
        public IActionResult UpdatePlaylist(int id, Playlist updatedPlaylist)
        {
            var playlist = _context.Playlists.Find(id);

            if (playlist == null)
                return NotFound();

            playlist.Name = updatedPlaylist.Name;
            playlist.OwnerId = updatedPlaylist.OwnerId;
            playlist.IsPublic = updatedPlaylist.IsPublic;

            _context.SaveChanges();

            return Ok(playlist);
        }

        // DELETE: api/playlists/5
        [HttpDelete("{id:int}")]
        public IActionResult DeletePlaylist(int id)
        {
            var playlist = _context.Playlists.Find(id);

            if (playlist == null)
                return NotFound();

            _context.Playlists.Remove(playlist);
            _context.SaveChanges();

            return Ok();
        }

        // POST: api/playlists/{playlistId}/tracks
        [HttpPost("{playlistId:int}/tracks")]
        public IActionResult AddTrackToPlaylist(int playlistId, int trackId,int? order)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            var track = _context.Tracks.Find(trackId);

            if (playlist == null || track == null)
                return NotFound();

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = playlistId,
                TrackId = trackId,
                Order = order ?? (playlist.Tracks.Count + 1),
                AddedAt = DateTime.UtcNow
            };
            playlist.Tracks.Add(playlistTrack);
            _context.SaveChanges();

            return Ok(playlist);
        }

        // DELETE: api/playlists/{playlistId}/tracks/{trackId}
        [HttpDelete("{playlistId:int}/tracks/{trackId:int}")]
        public IActionResult RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            
            var playlistTrack = _context.PlaylistTracks
                .FirstOrDefault(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);

            if (playlistTrack == null)
                return NotFound();

            _context.PlaylistTracks.Remove(playlistTrack);
            _context.SaveChanges();

            
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            return Ok(playlist);
        }

    }
}
