using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesDatabase.DatabaseModel.ManyToManyTables
{
    /// <summary>
    /// Представляет собой смежную таблицу между фильмами и странами для реализации отношения многое ко многим.
    /// </summary>
    [Table("VideosCountries")]
    public class VideoCountry
    {
        public ulong VideoId { get; set; }
        public Video Video { get; set; }
        public ulong CountryId { get; set; }
        public Country Country { get; set; }
    }
}
