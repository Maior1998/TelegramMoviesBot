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
    public abstract class SelectCountryStage : BotStage
    {
        private const string PleaseEnterText = "Please enter name of the country or use /cancel command";
        public SelectCountryStage([NotNull] ITelegramBotClient client) : base(client)
        {
        }
        public override void OnCallbackQuery(long userId, string message)
        {
            OnMessage(userId, message);
        }
        public override async void OnMessage(long userId, string message)
        {
            if (message.StartsWith("/"))
            {
                switch (message)
                {
                    case "/cancel":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok! Cancelled.");
                        OnStageChangingNeeded(new MainMenuBotStage(botClient));
                        break;
                    default:
                        await botClient.SendTextMessageAsync(chatId: userId, text: PleaseEnterText);
                        return;
                }
            }
            else
            {
                DatabaseContext db = new DatabaseContext();

                Country[] countries = db.Countries.Where(x => x.Name.ToLower() == message.ToLower()).ToArray();
                if (!countries.Any())
                {
                    await botClient.SendTextMessageAsync(chatId: userId, text: $"Sorry, i can't find a country with that name. {PleaseEnterText}");
                    return;
                }
                if (countries.Length > 1)
                {
                    await botClient.SendTextMessageAsync(chatId: userId, text: $"Ambigous name! Possible variants is:\n{string.Join(", ", countries.Select(x => x.Name).OrderBy(x => x))}\n\n{PleaseEnterText}");
                    return;
                }
                Country country = countries[0];
                OnCountryFound(db, userId, country);
            }
        }
            protected abstract void OnCountryFound(DatabaseContext db, long userId, Country country);
  
    }
}
