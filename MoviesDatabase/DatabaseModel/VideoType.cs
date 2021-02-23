using System;
namespace MoviesDatabase.DatabaseModel
{
    /// <summary>
    /// Представляет перечисление возможных типов видео в базах данных.
    /// </summary>
    [Flags]
    public enum VideoType
    {
        /// <summary>
        /// Фильм.
        /// </summary>
        Movie,
        /// <summary>
        /// Сериал.
        /// </summary>
        TvSeries
    }
}
