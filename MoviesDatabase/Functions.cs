using System;
using System.Linq;
using MoviesDatabase.DatabaseModel;

namespace MoviesDatabase
{
    public static class Functions
    {
        public static readonly Genre[] Genres = new[]
            {
                new Genre(){Name = "боевик и приключения", Type=VideoType.TvSeries},
new Genre(){Name = "мультфильм", Type=VideoType.TvSeries},
new Genre(){Name = "комедия", Type=VideoType.TvSeries},
new Genre(){Name = "криминал", Type=VideoType.TvSeries},
new Genre(){Name = "документальный", Type=VideoType.TvSeries},
new Genre(){Name = "драма", Type=VideoType.TvSeries},
new Genre(){Name = "семейный", Type=VideoType.TvSeries},
new Genre(){Name = "детский", Type=VideoType.TvSeries},
new Genre(){Name = "детектив", Type=VideoType.TvSeries},
new Genre(){Name = "новости", Type=VideoType.TvSeries},
new Genre(){Name = "реалити-шоу", Type=VideoType.TvSeries},
new Genre(){Name = "нф и фэнтези", Type=VideoType.TvSeries},
new Genre(){Name = "мыльная опера", Type=VideoType.TvSeries},
new Genre(){Name = "ток-шоу", Type=VideoType.TvSeries},
new Genre(){Name = "война и политика", Type=VideoType.TvSeries},
new Genre(){Name = "вестерн", Type=VideoType.TvSeries},

new Genre(){Name = "боевик", Type=VideoType.Movie},
new Genre(){Name = "приключения", Type=VideoType.Movie},
new Genre(){Name = "мультфильм", Type=VideoType.Movie},
new Genre(){Name = "комедия", Type=VideoType.Movie},
new Genre(){Name = "криминал", Type=VideoType.Movie},
new Genre(){Name = "документальный", Type=VideoType.Movie},
new Genre(){Name = "драма", Type=VideoType.Movie},
new Genre(){Name = "семейный", Type=VideoType.Movie},
new Genre(){Name = "фэнтези", Type=VideoType.Movie},
new Genre(){Name = "история", Type=VideoType.Movie},
new Genre(){Name = "ужасы", Type=VideoType.Movie},
new Genre(){Name = "музыка", Type=VideoType.Movie},
new Genre(){Name = "детектив", Type=VideoType.Movie},
new Genre(){Name = "мелодрама", Type=VideoType.Movie},
new Genre(){Name = "фантастика", Type=VideoType.Movie},
new Genre(){Name = "телевизионный фильм", Type=VideoType.Movie},
new Genre(){Name = "триллер", Type=VideoType.Movie},
new Genre(){Name = "военный", Type=VideoType.Movie},
new Genre(){Name = "вестерн", Type=VideoType.Movie},
            };
    }
}
