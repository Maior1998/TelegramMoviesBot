using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramMoviesBot.Model.TelegramApiFunctions
{
    public class TelegramApiFunctions
    {
        private static string apiKey;
        static ITelegramBotClient botClient;
        public static void start()
        {
            apiKey = File.ReadAllText("api.txt");
            botClient = new TelegramBotClient(apiKey);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            long chatId = e.Message.Chat.Id;
            DatabaseContext db = new DatabaseContext();
            User user = db.Users.Include(x=>x.Settings).SingleOrDefault(x => x.ApiIdentifier == chatId);
            if (e.Message.Text != null)
            {
                switch (e.Message.Text)
                {
                    case "/start":
                        
                        if (user == null)
                        {
                            user = new User()
                            {
                                ApiIdentifier = chatId,
                            };
                            db.Users.Add(user);
                            db.SaveChanges();
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Welcome to MovieBot!");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Welcome back!");
                        }
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"Bot is currently {(user.Settings.IsEnabled ? "enabled" : "disabled")} for you. You can change this by /disable and /enable commands!");
                        break;
                    case "/enable":
                        if (user.Settings.IsEnabled)
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"MoviesBot is already enabled for you.");
                        }
                        else
                        {
                            user.Settings.IsEnabled = true;
                            db.SaveChanges();
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"MoviesBot is now enabled for you. Congratulations!");
                        }
                        break;
                    case "/disable":
                        if (user.Settings.IsEnabled)
                        {
                            user.Settings.IsEnabled = false;
                            db.SaveChanges();
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"MoviesBot is now disabled for you.");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: $"MoviesBot is already disabled for you.");
                        }
                        break;
                }
            }
        }

        

        public static void SendMessage(User user, Video video)
        {
            string message = $"Название: {video.Name}\nОписание: {video.Description}\nДата выхода: {video.ReleaseDate.ToShortDateString()}";
            SendMessage(user, message);
        }

        public static void SendMessage(User user, string message)
        {
            botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: message);
        }

        

    }
}
