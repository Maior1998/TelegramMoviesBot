using System;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase.DatabaseModel;

namespace MoviesDatabase
{
    public class DatabaseContext : DbContext
    {

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./tgmovies.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(x => x.Settings)
                .WithOne(x => x.User)
                .IsRequired();


        }
    }
}
