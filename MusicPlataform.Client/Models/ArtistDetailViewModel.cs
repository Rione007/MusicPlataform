namespace MusicPlataform.Client.Models
{
    public class ArtistDetailViewModel
    {
        public ArtistClient Artista { get; set; }
        public IEnumerable<TrackClient> Tracks { get; set; }
    }
}
