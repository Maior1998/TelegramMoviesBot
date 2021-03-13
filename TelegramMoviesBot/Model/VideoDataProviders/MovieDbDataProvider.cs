using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Drawing;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Newtonsoft.Json;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

namespace TelegramMoviesBot.Model.VideoDataProviders
{
    public class MovieDbDataProvider : IVideoDataProvider
    {
        public string Name => "The Movie Database";


        private static MovieDbConfigEntry config;

        public Video[] GetNewVideos()
        {
            string genresJson = GetUrl($"https://api.themoviedb.org/3/genre/movie/list?api_key={BotSettings.MovieDbApiKey}&language=ru-RU");
            MovieDbGenresVideos genresParsed = JsonConvert.DeserializeObject<MovieDbGenresVideos>(genresJson);
            movieDbMovieGenres = genresParsed.genres.ToDictionary(x => x.id, x => x.name);
            genresJson = GetUrl($"https://api.themoviedb.org/3/genre/tv/list?api_key={BotSettings.MovieDbApiKey}&language=ru-RU");
            genresParsed = JsonConvert.DeserializeObject<MovieDbGenresVideos>(genresJson);
            movieDbTvSeriesGenres = genresParsed.genres.ToDictionary(x => x.id, x => x.name);



            int page = 1;
            Console.WriteLine("Downloading configuration");
            string configJson = GetUrl($"https://api.themoviedb.org/3/configuration?api_key={BotSettings.MovieDbApiKey}");
            config = JsonConvert.DeserializeObject<MovieDbConfigEntry>(configJson);
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={BotSettings.MovieDbApiKey}&language=ru-RU&sort_by=release_date.asc&include_adult=true&include_video=false&page={page}&release_date.gte={DateTime.Today:yyyy-MM-dd}";
            List<Video> videos = new List<Video>();
            string firstPage = GetUrl(url);
            MovieDbPageEntry movieDbPageEntry = JsonConvert.DeserializeObject<MovieDbPageEntry>(firstPage);
            int length = movieDbPageEntry.total_pages;
#if DEBUG
            length = 70;
#endif
            for (page = 1; page <= length; page++)
            {
                Thread.Sleep(100);
                Console.WriteLine($"Downloading page {page} from {length}");
                url = $"https://api.themoviedb.org/3/discover/movie?api_key={BotSettings.MovieDbApiKey}&language=ru-RU&sort_by=release_date.asc&include_adult=true&include_video=false&page={page}&release_date.gte={DateTime.Today:yyyy-MM-dd}";
                movieDbPageEntry = JsonConvert.DeserializeObject<MovieDbPageEntry>(GetUrl(url));
                videos.AddRange(movieDbPageEntry.results.Select(x => ConvertToVideo(x, VideoType.Movie)).Where(x => x.ReleaseDate.Date >= DateTime.Today));
            }
            return videos.Where(x => !string.IsNullOrWhiteSpace(x.Description)).ToArray();
        }

        public class MovieDbGenresVideos
        {
            public MovieDbGenresVideosEntry[] genres { get; set; }
        }

        public class MovieDbGenresVideosEntry
        {
            public int id { get; set; }
            public string name { get; set; }
        }


        private static string GetUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            string response;
            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                responseReader.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                return string.Empty;
            }
            return response;
        }
        private static string GetImage(string poster_path)
        {
            string url = $"{config.images.base_url}{config.images.poster_sizes[config.images.poster_sizes.Length / 2]}{poster_path}";
            return url;
        }

        private static Dictionary<int, string> movieDbMovieGenres = new Dictionary<int, string>();
        private static Dictionary<int, string> movieDbTvSeriesGenres = new Dictionary<int, string>();

        private static Video ConvertToVideo(MovieDbVideoEntry source, VideoType type)
        {
            Video result = new Video
            {
                Name = source.title,
                Description = source.overview,
                VotesAverage = source.vote_average,
                VotesCount = source.vote_count
            };
            DatabaseContext db = new DatabaseContext();
            foreach (int genreId in source.genre_ids)
            {
                Genre genre = null;
                if (type == VideoType.Movie)
                    genre = db.Genres.Where(x => x.Type == type).FirstOrDefault(x => x.Name.ToLower() == movieDbMovieGenres[genreId]);
                else
                    genre = db.Genres.Where(x => x.Type == type).FirstOrDefault(x => x.Name.ToLower() == movieDbTvSeriesGenres[genreId]);
                if (genre == null) continue;
                result.Genres.Add(new VideoGenre() { Genre = genre, GenreId = genre.Id });
            }
            string[] dateParts = source.release_date.Split("-");
            int year = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int day = int.Parse(dateParts[2]);
            result.ReleaseDate = new DateTime(year, month, day);
            if (source.poster_path != null)
                result.ImageUrl = GetImage(source.poster_path);
            return result;
        }

        private class MovieDbVideoEntry
        {
            public bool adult { get; set; }
            public string backdrop_path { get; set; }
            public int[] genre_ids { get; set; }
            public int id { get; set; }
            public string original_language { get; set; }
            public string original_title { get; set; }
            public string overview { get; set; }
            public float popularity { get; set; }
            public string poster_path { get; set; }
            public string release_date { get; set; }
            public string title { get; set; }
            public bool video { get; set; }
            public float vote_average { get; set; }
            public int vote_count { get; set; }
        }
        private class MovieDbPageEntry
        {
            public int page { get; set; }
            public MovieDbVideoEntry[] results { get; set; }
            public int total_pages { get; set; }
            public int total_results { get; set; }
        }

        private class MovieDbConfigEntry
        {
            public MovieDbImageConfigEntry images { get; set; }
            public string[] change_keys { get; set; }
        }

        private class MovieDbImageConfigEntry
        {
            public string base_url { get; set; }
            public string secure_base_url { get; set; }
            public string[] backdrop_sizes { get; set; }
            public string[] logo_sizes { get; set; }
            public string[] poster_sizes { get; set; }
            public string[] profile_sizes { get; set; }
            public string[] still_sizes { get; set; }


        }
    }
}
