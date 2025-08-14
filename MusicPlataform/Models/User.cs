
namespace MusicPlataform.Server.Models
    {
        public class User
        {
            public int Id { get; set; }

            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty; 
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
            public ICollection<TrackLike> LikedTracks { get; set; } = new List<TrackLike>();
        }
}

