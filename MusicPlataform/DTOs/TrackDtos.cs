namespace MusicPlataform.Server.DTOs
{
    public class TrackDtos
    {

        public record TrackCreateDto(string Title, int DurationSeconds, int ArtistId, int AlbumId, int? GenreId, string? AudioUrl);
        public record TrackReadDto(int Id, string Title, int DurationSeconds, string Artist, string Album, string? Genre, string? AudioUrl);

    }
}
