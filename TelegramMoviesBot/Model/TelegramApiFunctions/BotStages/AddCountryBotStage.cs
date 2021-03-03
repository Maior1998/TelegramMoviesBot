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
    public class AddCountryBotStage : SelectCountryStage
    {
        public AddCountryBotStage([NotNull] ITelegramBotClient client) : base(client)
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

        protected override async void OnCountryFound(DatabaseContext db, long userId, Country country)
        {
            User user = db.Users
                    .Include(x => x.Settings)
                    .ThenInclude(x => x.Countries)
                    .ThenInclude(x => x.Country)
                    .Single(x => x.ApiIdentifier == userId);
            if (user.Settings.Countries.Any(x => x.Country.Name.ToLower() == country.Name.ToLower()))
            {
                await botClient.SendTextMessageAsync(chatId: userId, text: "You already have this country in your wishlist!");
                OnStageChangingNeeded(new MainMenuBotStage(botClient));
            }
            user.Settings.Countries.Add(new SettingCountry() { Country = country });
            db.SaveChanges();
            await botClient.SendTextMessageAsync(chatId: userId, text: $"Country {country.Name} was successfully added to your wishlist!");
            OnStageChangingNeeded(new MainMenuBotStage(botClient));
        }
    }
}
