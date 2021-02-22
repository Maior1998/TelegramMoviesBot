using System;
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
        public bool IsEnabled { get; set; }

        public List<SettingGenre> Genres { get; set; }
        public List<SettingCountry> Countries { get; set; }
        public VideoType FilteringVideoType { get; set; }


    }
}
