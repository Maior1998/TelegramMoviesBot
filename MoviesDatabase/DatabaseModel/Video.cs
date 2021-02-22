using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет собой видео, будь то фильм или сериал.
    /// </summary>
    [Table("Viedos")]
    public class Video
    {
        /// <summary>
        /// Номер записи в базе данных.
        /// </summary>
        [Key]
        public ulong Id { get; set; }
        /// <summary>
        /// Заглавие видео.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата выхода видео.
        /// </summary>
        public DateTime RealeseDate { get; set; }
        /// <summary>
        /// Тип видео.
        /// </summary>
        public VideoType Type { get; set; }
        /// <summary>
        /// Список стран, в которых это видео выйдет\вышло.
        /// </summary>
        public List<VideoCountry> RealisedInCountries { get; set; } = new List<VideoCountry>();
        /// <summary>
        /// Список жанров этого видео.
        /// </summary>
        public List<VideoGenre> Genres { get; set; } = new List<VideoGenre>();
    }
}
