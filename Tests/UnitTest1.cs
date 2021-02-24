using MoviesDatabase;
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
            VideoProcessor videoProcessor = new VideoProcessor(new TestVideoProvider());
            videoProcessor.Start();
            Assert.Pass();
        }
    }
}