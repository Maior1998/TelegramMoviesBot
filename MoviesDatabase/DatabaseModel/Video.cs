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
    [Table("Videos")]
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
        /// Описание к видео.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата выхода видео.
        /// </summary>
        public DateTime ReleaseDate { get; set; }
        /// <summary>
        /// Тип видео.
        /// </summary>
        public VideoType Type { get; set; }

        /// <summary>
        /// Ссылка на трейлер к этому видео.
        /// </summary>
        public string TrailerUrl { get; set; }

        /// <summary>
        /// Обложка фильма\сериала.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Список жанров этого видео.
        /// </summary>
        public List<VideoGenre> Genres { get; set; } = new List<VideoGenre>();

        /// <summary>
        /// Число голосов рейтинга этого видео.
        /// </summary>
        public int VotesCount { get; set; }

        /// <summary>
        /// Средний рейтинг пользовательских голосов этого видео.
        /// </summary>
        public float VotesAverage { get; set; }
    }
}
