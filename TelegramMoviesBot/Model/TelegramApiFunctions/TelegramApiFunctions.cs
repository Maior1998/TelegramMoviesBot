﻿using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMoviesBot.Model.TelegramApiFunctions.BotStages;

namespace TelegramMoviesBot.Model.TelegramApiFunctions
{
    public class TelegramApiFunctions
    {
        
        private static string apiKey;
        static ITelegramBotClient botClient;
        private static BotStage CurrentStage;

        private static void ChangeStage(BotStage newStage)
        {
            if(CurrentStage != null)
            {
                botClient.OnMessage -= CurrentStage.OnMessage;
                botClient.OnInlineResultChosen -= CurrentStage.OnInlineResultChosen;
                botClient.OnInlineQuery -= CurrentStage.OnInlineQuery;
                botClient.OnCallbackQuery -= CurrentStage.OnCallbackQuery;
                CurrentStage.StageChangingNeeded -= ChangeStage;
            }

            CurrentStage = newStage;

            botClient.OnMessage += CurrentStage.OnMessage;
            botClient.OnInlineResultChosen += CurrentStage.OnInlineResultChosen;
            botClient.OnInlineQuery += CurrentStage.OnInlineQuery;
            botClient.OnCallbackQuery += CurrentStage.OnCallbackQuery;
            CurrentStage.StageChangingNeeded += ChangeStage;
        }


        public static void Start()
        {
            apiKey = File.ReadAllText("api.txt");
            botClient = new TelegramBotClient(apiKey);
            ChangeStage(new MainMenuBotStage(botClient));
            botClient.StartReceiving();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            botClient.StopReceiving();
        }

        public static void SendMessage(User user, Video video)
        {
            string message = $"Caption: {video.Name}\nDescription: {video.Description}\nRelease Date : {video.ReleaseDate.ToShortDateString()}";
            SendMessage(user, message);
        }

        public static void SendMessage(User user, string message)
        {
            botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: message);
        }



    }
}