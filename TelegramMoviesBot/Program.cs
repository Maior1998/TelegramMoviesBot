using System;

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
        }
    }
}
