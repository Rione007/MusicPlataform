namespace MusicPlataform.Server.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public int AlbumId { get; set; }
        public Album? Album { get; set; }

        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }

        public string? AudioUrl { get; set; } 

        public ICollection<PlaylistTrack> Playlists { get; set; } = new List<PlaylistTrack>();
        public ICollection<TrackLike> Likes { get; set; } = new List<TrackLike>();
    }
}
