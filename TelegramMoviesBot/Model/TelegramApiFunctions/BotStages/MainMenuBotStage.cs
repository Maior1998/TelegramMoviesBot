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
            "/start - Show main info page\n" +
            "/enable \\ /disable - Enables or disables the bot\n" +
            "/settings - show a settings page";

        public MainMenuBotStage([NotNull] ITelegramBotClient client) : base(client)
        {
        }

        public override async void OnMessage(long userId, string message)
        {
            DatabaseContext db = new DatabaseContext();
            User user = db.Users
                .Include(x => x.Settings)
                .ThenInclude(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
                .Include(x => x.Settings)
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
                InlineKeyboardMarkup inlineKeyboard = null;
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

                        inlineKeyboard = new InlineKeyboardMarkup(new[]
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
                    case "/setvotesaverage":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter votes average lower filter!");
                        OnStageChangingNeeded(new SetVotesAverageBotStage(botClient));
                        break;
                    case "/setvotescount":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter votes count lower filter!");
                        OnStageChangingNeeded(new SetVotesCountBotStage(botClient));
                        break;
                    case "/settings":

                        VideoType? videoType = user.Settings.FilteringVideoType;
                        string videoTypeFilterValue = !videoType.HasValue
                            ? "all"
                            : StaticFuncs.StaticFuncs.GetEnumDescription(videoType);
                        string genresFilterValue = !user.Settings.MovieGenres.Any()
                            ? "all"
                            : $" only {string.Join(", ", user.Settings.MovieGenres.Select(x => x.Genre.Name))}";
                        inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
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
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Set lower filter to votes average", "/setvotesaverage"),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Set lower filter to votes count", "/setvotescount"),
                            }
                        });
                        await botClient.SendTextMessageAsync(chatId: userId,
                            text: $"Ok, that's your settings:\n\n" +
                            $"Bot status: {(user.Settings.IsEnabled ? "enabled" : "disabled")}\n" +
                            $"Video types: {videoTypeFilterValue}\n" +
                            $"Genres: {genresFilterValue}\n"+
                            $"Votes count lower limit: {(user.Settings.VotesCount.HasValue && user.Settings.VotesCount!=0 ? user.Settings.VotesCount : "all")}\n"+
                            $"Votes average lower limit: {(user.Settings.VotesAverage.HasValue && user.Settings.VotesAverage != 0 ? user.Settings.VotesAverage : "all")}",
                            replyMarkup: inlineKeyboard);
                        break;
                    case "/addgenre":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Genre you want to add to your wishlist!");
                        OnStageChangingNeeded(new AddGenreStage(botClient));
                        break;
                    case "/removegenre":
                        await botClient.SendTextMessageAsync(chatId: userId, text: "Ok, now enter Name of the Genre you want to remove from your wishlist!");
                        OnStageChangingNeeded(new DeleteGenreStage(botClient));
                        break;
                    case "/cleargenres":
                        user.Settings.MovieGenres.Clear();
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
                string picUrl = null;
                if (user.Settings.IsEnabled)
                {
                    picUrl = "https://i.kym-cdn.com/entries/icons/original/000/034/581/Untitled-5.png";
                }
                else
                {
                    picUrl = "https://i.redd.it/jyfakluns6g51.jpg";
                }
                await botClient.SendPhotoAsync(chatId: user.ApiIdentifier, caption: $"MoviesBot is now {botStatus} for you.", photo: picUrl);
            }
        }

    }
}
