using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace MoviesDatabase.DatabaseModel
{
    [Table("Genres")]
    public class Genre
    {
        [Key] public ulong Id { get; set; }
        public string Name { get; set; }
        public List<SettingGenre> SettingsGenres { get; set; } = new List<SettingGenre>();
        public List<VideoGenre> Videos { get; set; }
            = new List<VideoGenre>();
    }
}
