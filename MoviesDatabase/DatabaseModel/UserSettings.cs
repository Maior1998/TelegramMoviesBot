﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет собой класс с основными настройками пользователя.
    /// </summary>
    [Table("UserSettings")]
    public class UserSettings
    {
        /// <summary>
        /// Номер записи в базе данных.
        /// </summary>
        [Key]
        public ulong Id { get; set; }
        /// <summary>
        /// Пользователь, к которому относятся данные настройки.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Определяет, включен ли бот у данного пользователя.
        /// </summary>
        public bool IsEnabled { get; set; } = false;
        /// <summary>
        /// Список жанров, которые предпочитает данный пользователь.
        /// </summary>
        public List<SettingGenre> MovieGenres { get; set; } = new List<SettingGenre>();
        /// <summary>
        /// Нижняя граница числа голосов рейтинга видео.
        /// </summary>
        public int? VotesCount { get; set; }
        /// <summary>
        /// Нижняя граница рейтинга видео.
        /// </summary>
        public float? VotesAverage { get; set; }

        /// <summary>
        /// Предпочитаемый список типов выходящего видео.
        /// </summary>
        public VideoType? FilteringVideoType { get; set; }


    }
}
