using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel.ManyToManyTables
{
    /// <summary>
    /// Представляет собой смежную таблицу между видео и жанрами для жанрами отношения многое ко многим.
    /// </summary>
    [Table("VideosGenres")]
    public class VideoGenre
    {
        public ulong VideoId { get; set; }
        public Video Video { get; set; }
        public ulong GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
