using System;
using System.IO;

namespace TelegramMoviesBot
{
    public static class BotSettings
    {
        static BotSettings()
        {
#if DEBUG
            telegramApiKey = File.ReadAllText("api_telegram.txt");
            movieDbApiKey = File.ReadAllText("api_moviedb.txt");
#endif
        }


        /// <summary>
        /// Интервал обновления фильмов (в минутах).
        /// </summary>
        public static uint UpdateInterval { get; set; } = 60 * 24;
        private static string telegramApiKey;
        public static string TelegramApiKey => telegramApiKey ??= Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
        private static string movieDbApiKey;
        public static string MovieDbApiKey => movieDbApiKey ??= Environment.GetEnvironmentVariable("TMDB_API_KEY");
    }
}
