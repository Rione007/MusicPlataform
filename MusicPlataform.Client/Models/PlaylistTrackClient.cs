namespace MusicPlataform.Client.Models
{
    public class PlaylistTrackClient
    {
        public int PlaylistId { get; set; }
        public int TrackId { get; set; }
        public int Order { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
