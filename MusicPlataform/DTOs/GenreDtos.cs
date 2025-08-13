namespace MusicPlataform.Server.DTOs
{
    public class GenreDtos
    {
        public record GenreCreateDto(string Name);
        public record GenreReadDto(int Id, string Name);
    }
}
