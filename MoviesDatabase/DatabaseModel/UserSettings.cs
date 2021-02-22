using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет собой класс с основными настройками пользователя.
    /// </summary>
    [Table("UserSettings")]
    public class UserSettings
    {
        [Key]
        public ulong Id { get; set; }
        public User User { get; set; }
        /// <summary>
        /// Определяет, включен ли бот у данного пользователя.
        /// </summary>
        public bool IsEnabled { get; set; }

    }
}
