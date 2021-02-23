using System;
using MoviesDatabase.DatabaseModel;
using TelegramMoviesBot.Model.VideoDataProviders;

namespace TelegramMoviesBot.Model
{
    public class VideoProcessor
    {
        private IVideoDataProvider VideoProvider;

        public VideoProcessor(IVideoDataProvider videoProvider)
        {
            VideoProvider = videoProvider;
        }


        public void Start()
        {
            Video[] newVideos = VideoProvider.GetNewVideos();

        }

    }
}
