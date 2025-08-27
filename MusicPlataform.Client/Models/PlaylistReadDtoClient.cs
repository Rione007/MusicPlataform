namespace MusicPlataform.Client.Models
{
    public class PlaylistReadDtoClient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public bool IsPublic { get; set; }
        public List<int> TrackIds { get; set; } = new List<int>();
    }
}
