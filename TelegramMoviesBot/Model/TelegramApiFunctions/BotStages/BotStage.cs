using System;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramMoviesBot.Model.TelegramApiFunctions.BotStages
{
    public abstract class BotStage
    {
        public event Action<BotStage> StageChangingNeeded;
        protected ITelegramBotClient botClient;

        protected void OnStageChangingNeeded(BotStage newStage)
        {
            StageChangingNeeded?.Invoke(newStage);
        }


        public BotStage([NotNull] ITelegramBotClient client)
        {
            botClient = client;
        }

        public async void OnMessage(object sender, MessageEventArgs e)
        {
            OnMessage(e.Message.Chat.Id, e.Message.Text);
        }

        public async void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            OnCallbackQuery(e.CallbackQuery.From.Id, e.CallbackQuery.Data);
        }

        public abstract void OnCallbackQuery(long userId, string message);
        public abstract void OnMessage(long userId, string message);

    }
}
