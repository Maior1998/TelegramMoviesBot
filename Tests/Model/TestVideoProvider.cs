using System;
using System.Collections.Generic;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;
using TelegramMoviesBot.Model;
using TelegramMoviesBot.Model.VideoDataProviders;
namespace Tests.Model

{
    public class TestVideoProvider : IVideoDataProvider
    {
        public string Name => "Test Video Provider";

        public Video[] GetNewVideos()
        {
            return new[] {
                new Video()
                {
                    Type=VideoType.Movie,
                    Genres = new List<VideoGenre>()
                    {
                        new VideoGenre()
                        {
                            GenreId=1,
                        },
                        new VideoGenre()
                        {
                            GenreId=6,
                        },
                    }
                },
                
               
            };
        }
    }
}
