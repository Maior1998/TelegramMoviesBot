using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;

namespace TelegramMoviesBot.Model.TelegramApiFunctions.BotStages
{
    public class SetVotesCountBotStage : BotStage
    {
        private const string PleaseEnterText = "Please enter a number of a video votes count or use /cancel command";
        public SetVotesCountBotStage([NotNull] ITelegramBotClient client) : base(client)
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

                if (!int.TryParse(message, out int parsedVotesAverage))
                {
                    await botClient.SendTextMessageAsync(chatId: userId, text: $"Can't recognize number \"{message}\"! {PleaseEnterText}");
                    return;
                }

                User user = db.Users.Include(x => x.Settings).Single(x => x.ApiIdentifier == userId);
                if (parsedVotesAverage == 0)
                    user.Settings.VotesCount = null;
                else
                    user.Settings.VotesCount = parsedVotesAverage;
                db.SaveChanges();
                await botClient.SendTextMessageAsync(chatId: userId, text: $"Now your votes count lower border is {message}!");
                OnStageChangingNeeded(new MainMenuBotStage(botClient));

            }
        }
    }
}
