﻿using Microsoft.EntityFrameworkCore;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase
{
    public class DatabaseContext : DbContext
    {

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSettings> UsersSerttins { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./tgmovies.sqlite");
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
                .WithMany(x => x.SettingsGenres)
                .HasForeignKey(x => x.GenreId);
            modelBuilder.Entity<SettingGenre>()
                .HasOne(x => x.Settings)
                .WithMany(x => x.Genres)
                .HasForeignKey(x => x.SettingsId);

            //Settings - = > SettingCountry < = - Country
            modelBuilder.Entity<SettingCountry>()
                .HasKey(x => new { x.CountryId, x.SettingsId });
            modelBuilder.Entity<SettingCountry>()
                .HasOne(x => x.Country)
                .WithMany(x => x.UserSettings)
                .HasForeignKey(x => x.CountryId);
            modelBuilder.Entity<SettingCountry>()
                .HasOne(x => x.Settings)
                .WithMany(x => x.Countries)
                .HasForeignKey(x => x.SettingsId);


            //Video - = > VideoCountry < = - Country
            modelBuilder.Entity<VideoCountry>()
                .HasKey(x => new { x.CountryId, x.VideoId });
            modelBuilder.Entity<VideoCountry>()
                .HasOne(x => x.Country)
                .WithMany(x => x.Videos)
                .HasForeignKey(x => x.CountryId);
            modelBuilder.Entity<VideoCountry>()
                .HasOne(x => x.Video)
                .WithMany(x => x.RealisedInCountries)
                .HasForeignKey(x => x.VideoId);


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
