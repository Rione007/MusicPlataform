namespace MusicPlataform.Client.Models
{
    public class TrackClient
    {
        public string Title { get; set; }
        public int DurationSeconds { get; set; }
        public int ArtistId { get; set; }
        public int AlbumId { get; set; }
        public int? GenreId { get; set; }
        public string? AudioUrl { get; set; }
    }
}
