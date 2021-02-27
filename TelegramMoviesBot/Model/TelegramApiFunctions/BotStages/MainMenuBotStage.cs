using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramMoviesBot.Model.TelegramApiFunctions.BotStages
{
    public class MainMenuBotStage : BotStage
    {

        private const string helpText =
            "/start - Show main info page" +
            "/enable \\ /disable - Enables or disables the bot" +
            "/settings - show a settings page";

        public MainMenuBotStage([NotNull] ITelegramBotClient client) : base(client)
        {
        }

        public override async void OnMessage(long userId, string message)
        {
            DatabaseContext db = new DatabaseContext();
            User user = db.Users
                .Include(x => x.Settings)
                .ThenInclude(x => x.Genres)
                .ThenInclude(x => x.Genre)
                .Include(x => x.Settings)
                .ThenInclude(x => x.Countries)
                .ThenInclude(x => x.Country)
                .SingleOrDefault(x => x.ApiIdentifier == userId);
            if (user == null)
            {
                user = new User()
                {
                    ApiIdentifier = userId,
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            if (message != null)
            {
                InlineKeyboardMarkup inlineKeyboar = null;
                switch (message)
                {
                    case "/start":

                        string buttonCaption = null;
                        string buttonValue = null;
                        if (user.Settings.IsEnabled)
                        {
                            buttonCaption = "Disable the bot";
                            buttonValue = "/disable";
                        }
                        else
                        {
                            buttonCaption = "Enable the bot";
                            buttonValue = "/enable";
                        }

                        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(buttonCaption, buttonValue),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Show your settings", "/settings"),
                            }
                        });
                        await botClient.SendTextMessageAsync(
                            chatId: userId,
                            text: $"Bot is currently {(user.Settings.IsEnabled ? "enabled" : "disabled")} for you. You can change this via button below",
                            replyMarkup: inlineKeyboard
                        );
                        break;
                    case "/enable":
                        SetUserIsEnabledStatus(db, user, true);
                        break;
                    case "/disable":
                        SetUserIsEnabledStatus(db, user, false);
                        break;
                    case "/settings":

                        VideoType videoType = user.Settings.FilteringVideoType;
                        string videoTypeFilterValue = (int)videoType == 0
                            ? "all"
                            : StaticFuncs.StaticFuncs.GetEnumDescription(videoType);
                        string genresFilterValue = !user.Settings.Genres.Any()
                            ? "all"
                            : $" only {string.Join(", ", user.Settings.Genres.Select(x => x.Genre.Name))}";
                        string countriesFilterValue = !user.Settings.Countries.Any()
                            ? "all"
                            : $"only {string.Join(", ", user.Settings.Countries.Select(x => x.Country.Name))}";
                        inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Add country to the wishlist", "/addcountry"),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Remove a country from the wishlist", "/removecountry"),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Clear a countries wishlist", "/clearcountries"),
                            },
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Add genre to the wishlist", "/addgenre"),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Remove a genre from the wishlist", "/removegenre"),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Clear a genres wishlist", "/cleargenres"),
                            }
                        });
                        await botClient.SendTextMessageAsync(chatId: userId,
                            text: $"Ok, that's your settings:\n\n" +
                            $"Bot status: {(user.Settings.IsEnabled ? "enabled" : "disabled")}\n" +
                            $"Video types: {videoTypeFilterValue}\n" +
                            $"Genres: {genresFilterValue}\n" +
                            $"Countries: {countriesFilterValue}",
                            replyMarkup: inlineKeyboard);
                        break;
                    case "/addcountry":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Country you want to add to your wishlist!");
                        OnStageChangingNeeded(new AddCountryBotStage(botClient));
                        break;
                    case "/removecountry":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Country you want to remove from your wishlist!");
                        OnStageChangingNeeded(new DeleteCountryStage(botClient));
                        break;
                    case "/addgenre":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Genre you want to add to your wishlist!");
                        OnStageChangingNeeded(new AddGenreStage(botClient));
                        break;
                    case "/removegenre":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Genre you want to remove from your wishlist!");
                        OnStageChangingNeeded(new DeleteGenreStage(botClient));
                        break;
                    case "/clearcountries":
                        user.Settings.Countries.Clear();
                        db.SaveChanges();
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, you countries withlist is now empty!");
                        break;
                    case "/cleargenres":
                        user.Settings.Genres.Clear();
                        db.SaveChanges();
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, you genres withlist is now empty!");
                        break;
                    default:
                        await botClient.SendTextMessageAsync(chatId: userId, text: $"I don't know that command!\n\nPossible commands is:\n{helpText}");
                        break;
                }
            }
        }

        public override async void OnCallbackQuery(long userId, string message)
        {
            OnMessage(userId, message);
        }

        public override async void OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            Console.WriteLine();
        }

        public override async void OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {
            Console.WriteLine();
        }


        private async void SetUserIsEnabledStatus(DatabaseContext db, User user, bool newStatus)
        {
            if (user == null) return;
            bool curStatus = user.Settings.IsEnabled;
            string botStatus = newStatus ? "enabled" : "disabled";
            if (curStatus == newStatus)
            {
                await botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: $"MoviesBot is already {botStatus} for you!");
            }
            else
            {
                user.Settings.IsEnabled = newStatus;
                db.SaveChanges();
                await botClient.SendTextMessageAsync(chatId: user.ApiIdentifier, text: $"MoviesBot is now {botStatus} for you.");
            }
        }

    }
}
