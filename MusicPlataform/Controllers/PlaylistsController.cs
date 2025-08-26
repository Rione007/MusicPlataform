using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Data;
using MusicPlataform.Server.Models;
using static MusicPlataform.Server.DTOs.PlaylistDtos;

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
                .Select(p => new PlaylistReadDto(
                    p.Id,
                    p.Name,
                    p.OwnerId,
                    p.IsPublic,
                    p.Tracks.Select(pt => pt.TrackId)
                ))
                .ToList();

            return Ok(playlists);
        }

        // GET: api/playlists/5
        [HttpGet("{id:int}")]
        public IActionResult GetPlaylistById(int id)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .Where(p => p.Id == id)
                .Select(p => new PlaylistReadDto(
                    p.Id,
                    p.Name,
                    p.OwnerId,
                    p.IsPublic,
                    p.Tracks.Select(pt => pt.TrackId)
                ))
                .FirstOrDefault();

            if (playlist == null)
                return NotFound();

            return Ok(playlist);
        }

        // POST: api/playlists
        [HttpPost]
        public IActionResult CreatePlaylist(PlaylistCreateDto dto)
        {
            var playlist = new Playlist
            {
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                IsPublic = dto.IsPublic
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return Ok(new PlaylistReadDto(
                playlist.Id,
                playlist.Name,
                playlist.OwnerId,
                playlist.IsPublic,
                Enumerable.Empty<int>()
            ));
        }

        // PUT: api/playlists/5
        [HttpPut("{id:int}")]
        public IActionResult UpdatePlaylist(int id, PlaylistCreateDto dto)
        {
            var playlist = _context.Playlists.Find(id);
            if (playlist == null)
                return NotFound();

            playlist.Name = dto.Name;
            playlist.OwnerId = dto.OwnerId;
            playlist.IsPublic = dto.IsPublic;

            _context.SaveChanges();

            var trackIds = _context.PlaylistTracks
                .Where(pt => pt.PlaylistId == id)
                .Select(pt => pt.TrackId);

            return Ok(new PlaylistReadDto(
                playlist.Id,
                playlist.Name,
                playlist.OwnerId,
                playlist.IsPublic,
                trackIds
            ));
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

        // ✅ POST: api/playlists/{playlistId}/tracks
        [HttpPost("{playlistId:int}/tracks")]
        public IActionResult AddTrackToPlaylist(int playlistId, PlaylistAddTrackDto dto)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Id == playlistId);

            if (playlist == null)
                return NotFound(new { Message = "Playlist no encontrada" });

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = playlistId,
                TrackId = dto.TrackId,
                Order = dto.Order ?? (playlist.Tracks.Count + 1),
                AddedAt = DateTime.UtcNow
            };

            _context.PlaylistTracks.Add(playlistTrack);
            _context.SaveChanges();

            return Ok(new { Message = "Track agregado con éxito", playlistId, dto.TrackId, playlistTrack.Order });
        }

        // ✅ DELETE: api/playlists/{playlistId}/tracks/{trackId}
        [HttpDelete("{playlistId:int}/tracks/{trackId:int}")]
        public IActionResult RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            var playlistTrack = _context.PlaylistTracks
                .FirstOrDefault(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);

            if (playlistTrack == null)
                return NotFound(new { Message = "Track no encontrado en la playlist" });

            _context.PlaylistTracks.Remove(playlistTrack);
            _context.SaveChanges();

            return Ok(new { Message = "Track eliminado con éxito", playlistId, trackId });
        }

        // ✅ POST: api/playlists/user/{username}/library/addtrack
        // Adds the given track to the user's single "Mi Biblioteca" playlist.
        [HttpPost("user/{username}/library/addtrack")]
        public IActionResult AddTrackToUserLibrary(string username, [FromBody] PlaylistAddTrackDto dto)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { Message = "Username requerido" });

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado" });

            var track = _context.Tracks.Find(dto.TrackId);
            if (track == null)
                return NotFound(new { Message = "Track no encontrado" });

            // Use a single playlist named "Mi Biblioteca" for each user
            var libraryName = "Mi Biblioteca";
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.OwnerId == user.Id && p.Name == libraryName);

            if (playlist == null)
            {
                playlist = new Playlist
                {
                    Name = libraryName,
                    OwnerId = user.Id,
                    IsPublic = false
                };
                _context.Playlists.Add(playlist);
                _context.SaveChanges();
            }

            // Check if the track is already present in this library
            var existing = _context.PlaylistTracks
                .FirstOrDefault(pt => pt.PlaylistId == playlist.Id && pt.TrackId == dto.TrackId);

            if (existing != null)
                return BadRequest(new { Message = "Track ya agregado en la biblioteca del usuario" });

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = playlist.Id,
                TrackId = dto.TrackId,
                Order = (_context.PlaylistTracks.Count(pt => pt.PlaylistId == playlist.Id)) + 1,
                AddedAt = DateTime.UtcNow
            };

            _context.PlaylistTracks.Add(playlistTrack);
            _context.SaveChanges();

            return Ok(new { Message = "Track agregado a la biblioteca de usuario", playlistId = playlist.Id, trackId = dto.TrackId });
        }


        // GET: api/playlists/user/{username}
        [HttpGet("user/{username}")]
        public ActionResult<IEnumerable<PlaylistReadDto>> GetPlaylistsForUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest();

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return NotFound();

            var playlists = _context.Playlists
            .Include(p => p.Tracks) // ⚡ asegúrate de incluir la relación
            .Where(p => p.OwnerId == user.Id)
            .Select(p => new PlaylistReadDto(
                p.Id,
                p.Name,
                p.OwnerId,
                p.IsPublic,
                p.Tracks.Select(pt => pt.TrackId) // 👈 usa siempre "Tracks"
            ))
            .ToList();


            return Ok(playlists);
        }

        // ✅ GET: api/playlists/user/{username}/library
        // Devuelve todos los tracks de la biblioteca del usuario
        // ✅ GET: api/playlists/user/{username}/library
        // Devuelve todos los tracks de la biblioteca del usuario (solo nombre y audio)
        [HttpGet("user/{username}/library")]
        public IActionResult GetUserLibrary(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { Message = "Username requerido" });

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado" });

            var libraryName = "Mi Biblioteca";
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                    .ThenInclude(pt => pt.Track)
                .FirstOrDefault(p => p.OwnerId == user.Id && p.Name == libraryName);

            if (playlist == null)
                return Ok(new List<object>()); // si no existe aún, devolvemos vacío

            var tracks = playlist.Tracks
                .Select(pt => new {
                    Title = pt.Track.Title,
                    AudioUrl = pt.Track.AudioUrl
                })
                .ToList();

            return Ok(tracks);
        }



    }
}
