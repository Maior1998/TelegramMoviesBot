﻿using System;
namespace TelegramMoviesBot
{
    public class BotSettings
    {
        /// <summary>
        /// Интервал обновления фильмов (в минутах).
        /// </summary>
        public uint UpdateInterval { get; set; } = 60 * 24;
    }
}
