using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramMoviesBot.Model.TelegramApiFunctions.BotStages
{
    public class AddGenreStage : SelectGenreStage
    {
        public AddGenreStage([NotNull] ITelegramBotClient client) : base(client)
        {
        }


        public override void OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override async void OnGenreFound(DatabaseContext db, long userId, Genre genre)
        {
            User user = db.Users
                    .Include(x => x.Settings)
                    .ThenInclude(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                    .Single(x => x.ApiIdentifier == userId);
            if (user.Settings.Genres.Any(x => x.Genre.Name.ToLower() == genre.Name.ToLower()))
            {
                await botClient.SendTextMessageAsync(chatId: userId, text: "You already have this genre in your wishlist!");
                OnStageChangingNeeded(new MainMenuBotStage(botClient));
            }
            user.Settings.Genres.Add(new SettingGenre() { Genre = genre });
            db.SaveChanges();
            await botClient.SendTextMessageAsync(chatId: userId, text: $"Genre {genre.Name} was successfully added to your wishlist!");
            OnStageChangingNeeded(new MainMenuBotStage(botClient));
        }
    }
}
