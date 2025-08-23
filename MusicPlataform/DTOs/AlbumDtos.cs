namespace MusicPlataform.Server.DTOs
{
    public class AlbumDtos
    {
        // Para crear o actualizar un álbum
        public record AlbumCreateDto(string Title, int Year, int ArtistId, string? CoverUrl);

        // Para leer un álbum (sin incluir Tracks aún)
        public record AlbumReadDto(int Id, string Title, int Year, int ArtistId, string? CoverUrl);
    }
}
