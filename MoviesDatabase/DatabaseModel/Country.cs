using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет собой страну, в которой потенциально может выйти фильм или сериал.
    /// </summary>
    [Table("Countries")]
    public class Country
    {
        /// <summary>
        /// Номер записи в базе данных.
        /// </summary>
        [Key]
        public ulong Id { get; set; }
        /// <summary>
        /// Название данной страны.
        /// </summary>
        public string Name { get; set; }

        public List<SettingCountry> UserSettings { get; set; }
            = new List<SettingCountry>();

        public List<VideoCountry> Videos { get; set; }
            = new List<VideoCountry>();
    }
}
