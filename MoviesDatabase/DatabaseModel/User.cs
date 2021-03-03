using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет пользователя приложения.
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Номер записи в базе данных.
        /// </summary>
        [Key]
        public ulong Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя в API Telegram.
        /// </summary>
        [Required][Index]
        public long ApiIdentifier { get; set; }
        /// <summary>
        /// Настройки приложения данного пользователя.
        /// </summary>
        [Required]
        [ForeignKey("SettingsId")]
        public UserSettings Settings { get; set; } = new UserSettings();
    }
}
