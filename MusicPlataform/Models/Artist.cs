namespace MusicPlataform.Server.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
