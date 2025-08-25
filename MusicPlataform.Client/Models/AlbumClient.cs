namespace MusicPlataform.Client.Models
{
    public class AlbumClient
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public int ArtistId { get; set; }
        public string? CoverUrl { get; set; }
    }
}
