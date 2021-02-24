using System;
using MoviesDatabase.DatabaseModel;

namespace TelegramMoviesBot.Model.VideoDataProviders
{
    public interface IVideoDataProvider
    {
        public string Name { get; }
        public Video[] GetNewVideos();
    }
}
