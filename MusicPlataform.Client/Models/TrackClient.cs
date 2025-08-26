using System.Text.Json.Serialization;

namespace MusicPlataform.Client.Models
{
    public class TrackClient
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DurationSeconds { get; set; }
        public string? Artist { get; set; }
        public int ArtistId { get; set; }
        public string Genre { get; set; }
        public string? AudioUrl { get; set; }
    }
}
