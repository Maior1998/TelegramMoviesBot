using System;
using MoviesDatabase.DatabaseModel;

namespace TelegramMoviesBot.Model.VideoDataProviders
{
    public interface IVideoDataProvider
    {
        public Video[] GetNewVideos();
    }
}
