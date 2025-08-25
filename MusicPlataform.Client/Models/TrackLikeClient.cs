using System.ComponentModel.DataAnnotations;

namespace MusicPlataform.Server.Models
{
    public class TrackLikeClient
    {
        public int UserId { get; set; }
        public int TrackId { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
