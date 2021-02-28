using System;
using System.Linq;
using MoviesDatabase.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using TelegramMoviesBot.Model.TelegramApiFunctions;
using TelegramMoviesBot.Model;
using TelegramMoviesBot.Model.VideoDataProviders;
using System.Threading.Tasks;

namespace TelegramMoviesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MoviesDatabase.DatabaseContext db = new MoviesDatabase.DatabaseContext();

            Console.WriteLine("Deleting database . . .");
            db.Database.EnsureDeleted();
            Console.WriteLine("Creating database . . .");
            db.Database.EnsureCreated();
            Task.Run(()=>TelegramApiFunctions.Start());
            VideoProcessor videoProcessor = new VideoProcessor(new MovieDbDataProvider());
            videoProcessor.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            TelegramApiFunctions.Stop();
            Console.WriteLine();
        }
    }
}
