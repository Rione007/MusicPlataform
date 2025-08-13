namespace MusicPlataform.Server.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<PlaylistTrack> Tracks { get; set; } = new List<PlaylistTrack>();
        public bool IsPublic { get; set; } = true;
    }
}
