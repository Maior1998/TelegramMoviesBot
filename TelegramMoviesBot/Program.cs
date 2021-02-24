using System;
using System.Linq;
using MoviesDatabase.DatabaseModel;
using Microsoft.EntityFrameworkCore;

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
            User[] users = db.Users.ToArray();
            Console.WriteLine();
        }
    }
}
