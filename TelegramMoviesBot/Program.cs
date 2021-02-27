using System;
using System.Linq;
using MoviesDatabase.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using TelegramMoviesBot.Model.TelegramApiFunctions;

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
            

            TelegramApiFunctions.Start();
            Console.WriteLine();
        }
    }
}
