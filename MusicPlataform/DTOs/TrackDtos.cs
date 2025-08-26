namespace MusicPlataform.Server.DTOs
{
    public class TrackDtos
    {

        public record TrackCreateDto(string Title, int DurationSeconds, int ArtistId, int? GenreId, string? AudioUrl);
        public record TrackReadDto(int Id, string Title, int DurationSeconds,  string Artist, string? Genre, string? AudioUrl);

    }
}
