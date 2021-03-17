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


        private static string telegramApiKey;
        public static string TelegramApiKey
        {
            get
            {
                if (telegramApiKey != null) return telegramApiKey;
                telegramApiKey = Environment.GetEnvironmentVariable("TELEGRAM_API");
                File.WriteAllText("api_key.txt", telegramApiKey);
                return telegramApiKey;
            }
        }
        private static string movieDbApiKey;
        public static string MovieDbApiKey => movieDbApiKey ??= Environment.GetEnvironmentVariable("TMDB_API_KEY");
    }
}
