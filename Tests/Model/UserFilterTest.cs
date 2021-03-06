﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;
using NUnit.Framework;
using TelegramMoviesBot.Model;

namespace Tests.Model
{
    public class UserFilterTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestGenre()
        {
            DatabaseContext db = new DatabaseContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Genres.AddRange(Functions.Genres);
            db.SaveChanges();
            User testUser = new User()
            {
                Id = 7,
                ApiIdentifier = 325235,
                Settings = new UserSettings()
                {
                    IsEnabled = true,
                    FilteringVideoType = VideoType.Movie,
                    MovieGenres = new List<SettingGenre>()
                    {
                       new SettingGenre()
                       {
                           GenreId = 6
                       }
                    }
                }
            };
            Video video = new Video()
            {
                Type = VideoType.Movie,
                Genres = new List<VideoGenre>()
                {
                    new VideoGenre()
                    {
                        GenreId = 1,
                    },
                    new VideoGenre()
                    {
                        GenreId = 6,
                    },
                }
            };
            db.Users.Add(testUser);
            db.SaveChanges();
            User[] users = VideoProcessor.CheckUsers(db, video);
            bool isContains = users.Any(x => x.Id == testUser.Id);
            Assert.AreEqual(isContains, true);
        }
    }
}
