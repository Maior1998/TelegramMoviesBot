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
        public DateTime ReleaseDate { get; set; }
        /// <summary>
        /// Тип видео.
        /// </summary>
        public VideoType Type { get; set; }
        /// <summary>
        /// Список стран выхода этого видео.
        /// </summary>
        public List<VideoCountry> Countries { get; set; } = new List<VideoCountry>();

        /// <summary>
        /// Ссылка на трейлер к этому видео.
        /// </summary>
        public string TrailerUrl { get; set; }

        /// <summary>
        /// Обложка фильма\сериала.
        /// </summary>
        [NotMapped] public byte[] Image { get; set; }

        /// <summary>
        /// Список жанров этого видео.
        /// </summary>
        public List<VideoGenre> Genres { get; set; } = new List<VideoGenre>();
    }
}
