using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel.ManyToManyTables
{
    /// <summary>
    /// Представляет собой смежную таблицу между настройками и странами для реализации отношения многое ко многим.
    /// </summary>
    [Table("SettingsCountries")]
    public class SettingCountry
    {
        public ulong SettingsId { get; set; }
        public UserSettings Settings { get; set; }
        public ulong CountryId { get; set; }
        public Country Country { get; set; }
    }
}
