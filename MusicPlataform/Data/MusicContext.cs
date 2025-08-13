using Microsoft.EntityFrameworkCore;
using MusicPlataform.Server.Models;

namespace MusicPlataform.Server.Data
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<TrackLike> TrackLikes { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== PlaylistTrack =====================
            modelBuilder.Entity<PlaylistTrack>()
                .HasKey(pt => new { pt.PlaylistId, pt.TrackId });

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.Tracks)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Track)
                .WithMany(t => t.Playlists)
                .HasForeignKey(pt => pt.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== TrackLike ========================
            modelBuilder.Entity<TrackLike>()
                .HasKey(tl => new { tl.UserId, tl.TrackId });

            modelBuilder.Entity<TrackLike>()
                .HasOne(tl => tl.User)
                .WithMany(u => u.LikedTracks)
                .HasForeignKey(tl => tl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrackLike>()
                .HasOne(tl => tl.Track)
                .WithMany(t => t.Likes)
                .HasForeignKey(tl => tl.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== Artist -> Album ==================
            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== Artist -> Track ==================
            modelBuilder.Entity<Track>()
                .HasOne(t => t.Artist)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 Bloquea cascada

            // ====================== Album -> Track ===================
            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(al => al.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== Track -> Genre ===================
            modelBuilder.Entity<Track>()
                .HasOne(t => t.Genre)
                .WithMany(g => g.Tracks)
                .HasForeignKey(t => t.GenreId)
                .OnDelete(DeleteBehavior.SetNull);


            // ====================== Playlist -> User =================
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
