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
                    Countries = new List<VideoCountry>()
                    {
                        new VideoCountry()
                        {
                            CountryId=1
                        },
                        new VideoCountry()
                        {
                            CountryId=4
                        }
                        ,new VideoCountry()
                        {
                            CountryId=10
                        }
                    },
                    Genres = new List<VideoGenre>()
                    {
                        new VideoGenre()
                        {
                            GenreId=2,
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
