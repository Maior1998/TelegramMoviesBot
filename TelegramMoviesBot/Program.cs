using System;
using System.Linq;
using MoviesDatabase.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using TelegramMoviesBot.Model.TelegramApiFunctions;
using TelegramMoviesBot.Model;
using TelegramMoviesBot.Model.VideoDataProviders;
using System.Threading.Tasks;
using MoviesDatabase;

namespace TelegramMoviesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseContext db = new DatabaseContext();

            Console.WriteLine("Deleting database . . .");
            db.Database.EnsureDeleted();
            Console.WriteLine("Creating database . . .");
            db.Database.EnsureCreated();

            db.Genres.AddRange(Functions.Genres);
            db.SaveChanges();


            TelegramApiFunctions.Start();
            VideoProcessor videoProcessor = new VideoProcessor(new MovieDbDataProvider());
            Task videoProcessingTask = Task.Run(() => videoProcessor.Start());
            videoProcessingTask.Wait();
            TelegramApiFunctions.Stop();
        }
    }
}
