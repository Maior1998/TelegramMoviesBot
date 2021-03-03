using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel.ManyToManyTables
{
    /// <summary>
    /// Представляет собой смежную таблицу между настройками и жанрами фильмов для реализации отношения многое ко многим.
    /// </summary>
    [Table("SettingsGenres")]
    public class SettingGenre
    {
        public ulong SettingsId { get; set; }
        public UserSettings Settings { get; set; }
        public ulong GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
