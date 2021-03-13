using System;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase
{
    public class DatabaseContext : DbContext
    {
        public readonly ILoggerFactory MyLoggerFactory;
        public DatabaseContext() : base()
        {
            MyLoggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
            });
        }
        private static readonly string SettingsFileName = $"{nameof(SqlLiteSettings)}.json";
        private static void TryLoadSettings()
        {
            string settingsFileContent = File.ReadAllText(SettingsFileName);
            
        }

        private static void SaveSettings()
        {

        }

        public static SqlLiteSettings Settings { get; set; } = new SqlLiteSettings()
        {
            SettingsFilePath = "tgmovies.sqlite"
        };





        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSettings> UsersSettings { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite($"Filename={Settings.SettingsFilePath}")
                .UseLoggerFactory(MyLoggerFactory)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User - - - Settings
            modelBuilder.Entity<User>()
                .HasOne(x => x.Settings)
                .WithOne(x => x.User)
                .IsRequired();

            //Settings - = > SettingGenre < = - Genre
            modelBuilder.Entity<SettingGenre>()
                .HasKey(x => new { x.GenreId, x.SettingsId });
            modelBuilder.Entity<SettingGenre>()
                .HasOne(x => x.Genre)
                .WithMany(x => x.Settings)
                .HasForeignKey(x => x.GenreId);
            modelBuilder.Entity<SettingGenre>()
                .HasOne(x => x.Settings)
                .WithMany(x => x.MovieGenres)
                .HasForeignKey(x => x.SettingsId);


            //Video - = > VideoGenre < = - Genre
            modelBuilder.Entity<VideoGenre>()
                .HasKey(x => new { x.GenreId, x.VideoId });
            modelBuilder.Entity<VideoGenre>()
                .HasOne(x => x.Genre)
                .WithMany(x => x.Videos)
                .HasForeignKey(x => x.GenreId);
            modelBuilder.Entity<VideoGenre>()
                .HasOne(x => x.Video)
                .WithMany(x => x.Genres)
                .HasForeignKey(x => x.VideoId);


        }
    }
}
