using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Drawing;
using MoviesDatabase.DatabaseModel;
using Newtonsoft.Json;

namespace TelegramMoviesBot.Model.VideoDataProviders
{
    public class MovieDbDataProvider : IVideoDataProvider
    {
        public string Name => "The Movie Database";


        private static MovieDbConfigEntry config;

        public Video[] GetNewVideos()
        {
            int page = 1;
            string configJson = GetUrl($"https://api.themoviedb.org/3/configuration?api_key={BotSettings.MovieDbApiKey}");
            config = JsonConvert.DeserializeObject<MovieDbConfigEntry>(configJson);
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={BotSettings.MovieDbApiKey}&language=ru-RU&sort_by=release_date.asc&include_adult=true&include_video=false&page={page}&release_date.gte={DateTime.Today:yyyy-MM-dd}";
            Console.WriteLine("Downloading configuration");
            List<Video> videos = new List<Video>();
            string firstPage = GetUrl(url);
            MovieDbPageEntry movieDbPageEntry = JsonConvert.DeserializeObject<MovieDbPageEntry>(firstPage);
            for (page = 1; page <= movieDbPageEntry.total_pages; page++)
            {
                Thread.Sleep(500);
                Console.WriteLine($"Downloading page {page} from {movieDbPageEntry.total_pages}");
                url = $"https://api.themoviedb.org/3/discover/movie?api_key={BotSettings.MovieDbApiKey}&language=ru-RU&sort_by=release_date.asc&include_adult=true&include_video=false&page={page}&release_date.gte={DateTime.Today:yyyy-MM-dd}";
                movieDbPageEntry = JsonConvert.DeserializeObject<MovieDbPageEntry>(GetUrl(url));
                videos.AddRange(movieDbPageEntry.results.Select(x => ConvertToVideo(x)).Where(x => x.ReleaseDate.Date >= DateTime.Today));
            }
            return videos.ToArray();
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
        private static byte[] GetImage(string poster_path)
        {
            string url = $"{config.images.base_url}{config.images.poster_sizes[config.images.poster_sizes.Length / 2]}{poster_path}";

            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                return data;
            }
        }

        private static Video ConvertToVideo(MovieDbVideoEntry source)
        {
            Video result = new Video();

            result.Name = source.title;
            result.Description = source.overview;
            int year, month, day;
            string[] dateParts = source.release_date.Split("-");
            year = int.Parse(dateParts[0]);
            month = int.Parse(dateParts[1]);
            day = int.Parse(dateParts[2]);
            result.ReleaseDate = new DateTime(year, month, day);
            //TODO: скачать обложку
            if (source.poster_path != null)
               result.Image= GetImage(source.poster_path);
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
