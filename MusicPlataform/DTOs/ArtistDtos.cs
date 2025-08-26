namespace MusicPlataform.Server.DTOs
{
    public class ArtistDtos
    {
        public record ArtistCreateDto(string Name, string? Bio, string ImageUrl);
        public record ArtistReadDto(int Id, string Name, string? Bio, string? ImageUrl);
    }
}
