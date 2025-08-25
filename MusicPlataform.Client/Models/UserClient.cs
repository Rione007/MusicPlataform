
namespace MusicPlataform.Client.Models
    {
        public class UserClient
        {
            public int Id { get; set; }

            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty; 
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<PlaylistClient> Playlists { get; set; } = new List<PlaylistClient>();
        }
}

