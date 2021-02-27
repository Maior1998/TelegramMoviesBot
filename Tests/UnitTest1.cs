using System.Collections.Generic;
using System.Linq;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;
using NUnit.Framework;
using TelegramMoviesBot.Model;
using Tests.Model;

namespace Tests
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            DatabaseContext db = new DatabaseContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            User testUser = new User()
            {
                ApiIdentifier=325235,
                Settings = new UserSettings()
                {
                    IsEnabled=true,
                    FilteringVideoType=VideoType.Movie,
                    Genres = new List<SettingGenre>()
                    {
                       new SettingGenre()
                       {
                           Genre = new Genre()
                           {

                           }
                       }
                    }
                }
            };
            db.Users.Add(testUser);
            db.SaveChanges();
            Genre[] allgenres = db.Genres.ToArray();
            VideoProcessor videoProcessor = new VideoProcessor(new TestVideoProvider());
            videoProcessor.Start();
            Assert.Pass();
        }
    }
}