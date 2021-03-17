using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMoviesBot.Model.TelegramApiFunctions.BotStages;

namespace TelegramMoviesBot.Model.TelegramApiFunctions
{
    public class TelegramApiFunctions
    {
        
        static ITelegramBotClient botClient;
        private static BotStage CurrentStage;

        private static void ChangeStage(BotStage newStage)
        {
            if(CurrentStage != null)
            {
                botClient.OnMessage -= CurrentStage.OnMessage;
                botClient.OnCallbackQuery -= CurrentStage.OnCallbackQuery;
                CurrentStage.StageChangingNeeded -= ChangeStage;
            }

            CurrentStage = newStage;

            botClient.OnMessage += CurrentStage.OnMessage;
            botClient.OnCallbackQuery += CurrentStage.OnCallbackQuery;
            CurrentStage.StageChangingNeeded += ChangeStage;
        }

        public static void Stop()
        {
            botClient.StopReceiving();
        }

        public static void Start()
        {
            botClient = new TelegramBotClient(BotSettings.TelegramApiKey);
            ChangeStage(new MainMenuBotStage(botClient));
            botClient.StartReceiving();
        }

        public static void SendMessage(User user, Video video)
        {
            string message = $"<b>Caption</b>: {video.Name}\n" +
                $"<b>Description</b>: {video.Description}\n" +
                $"Release Date : {video.ReleaseDate.ToShortDateString()}\n" +
                $"Votes count: {video.VotesCount}\n" +
                $"Votes average: {video.VotesAverage}";
            if (string.IsNullOrWhiteSpace(video.ImageUrl))
                botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: message, parseMode: ParseMode.Html);
            else
                botClient.SendPhotoAsync(chatId: user.ApiIdentifier, photo: video.ImageUrl, caption: message, parseMode: ParseMode.Html);
        }

        public static void SendMessage(User user, string message)
        {
            botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: message);
        }



    }
}
