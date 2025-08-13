namespace MusicPlataform.Server.DTOs
{
    public class PlaylistDtos
    {
        public record PlaylistCreateDto(string Name, int OwnerId, bool IsPublic);
        public record PlaylistReadDto(int Id, string Name, int OwnerId, bool IsPublic, IEnumerable<int> TrackIds);
        public record PlaylistAddTrackDto(int TrackId, int? Order);

    }
}
