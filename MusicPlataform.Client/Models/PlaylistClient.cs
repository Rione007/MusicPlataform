namespace MusicPlataform.Client.Models
{
    public class PlaylistClient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        
        public bool IsPublic { get; set; } = true;
    }
}
