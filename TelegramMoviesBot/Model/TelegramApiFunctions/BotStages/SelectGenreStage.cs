using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;

namespace TelegramMoviesBot.Model.TelegramApiFunctions.BotStages
{
    public abstract class SelectGenreStage : BotStage
    {
        private const string PleaseEnterText = "Please enter name of the genre or use /cancel command";

        public SelectGenreStage([NotNull] ITelegramBotClient client) : base(client)
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

                Genre[] genres = db.Genres.Where(x => x.Name.ToLower() == message.ToLower()).ToArray();
                if (!genres.Any())
                {
                    await botClient.SendTextMessageAsync(chatId: userId, text: $"Sorry, i can't find a genre with that name. {PleaseEnterText}");
                    return;
                }
                if (genres.Length > 1)
                {
                    await botClient.SendTextMessageAsync(chatId: userId, text: $"Ambigous name! Possible variants is:\n{string.Join(", ", genres.Select(x => x.Name).OrderBy(x => x))}\n\n{PleaseEnterText}");
                    return;
                }
                Genre genre = genres[0];
                OnGenreFound(db, userId, genre);
            }
        }
        protected abstract void OnGenreFound(DatabaseContext db, long userId, Genre genre);

    }
}
