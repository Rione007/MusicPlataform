namespace MusicPlataform.Server.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
        public string? CoverUrl { get; set; }
    }
}
