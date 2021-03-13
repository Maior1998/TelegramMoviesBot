using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;

using TelegramMoviesBot.Model.VideoDataProviders;

namespace TelegramMoviesBot.Model
{
    public class VideoProcessor
    {
        private IVideoDataProvider VideoProvider;
        public ushort UpdateIntervalInMinutes;
        public VideoProcessor(IVideoDataProvider videoProvider, ushort updateIntervalInMinutes = 60 * 24)
        {
            VideoProvider = videoProvider;
            UpdateIntervalInMinutes = updateIntervalInMinutes;

        }



        public void Start()
        {
#if DEBUG
            PerformUpdate();
#endif
            while (true)
            {
                Thread.Sleep(1000 * 60 * UpdateIntervalInMinutes);
                PerformUpdate();
            }


        }



        private async void PerformUpdate()
        {
            await Task.Run(() =>
            {
                try
                {
                    DatabaseContext databaseContext = new DatabaseContext();
                    Video[] newVideos = VideoProvider.GetNewVideos().ToArray();
                    string[] curNames = databaseContext.Videos.Select(x => x.Name).ToArray();
                    newVideos = newVideos.Where(x => !curNames.Contains(x.Name?.ToLower())).ToArray();
                    curNames = null;
                    foreach (Video video in newVideos)
                    {
                        User[] usersWithNotifications = CheckUsers(databaseContext, video);
                        foreach (User notifyUser in usersWithNotifications)
                        {
                            TelegramApiFunctions.TelegramApiFunctions.SendMessage(notifyUser, video);
                        }
                        video.Genres = new System.Collections.Generic.List<VideoGenre>(video.Genres.Select(x => new VideoGenre() { GenreId = x.GenreId }));
                        databaseContext.Videos.Add(video);
                        databaseContext.SaveChanges();
                    }
                    Video[] oldVideos = databaseContext.Videos.Where(x => x.ReleaseDate < DateTime.Today).ToArray();
                    databaseContext.Videos.RemoveRange(oldVideos);
                    databaseContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ocurred while updating a videos! {ex.Message}\n{ex.StackTrace}");
                }
            });
        }



        public static User[] CheckUsers(DatabaseContext db, Video video)
        {
            ParameterExpression param = Expression.Parameter(typeof(User), "x");
            Expression exp = Expression.Equal(
            Expression.Property(
                Expression.Property(param, nameof(User.Settings)), nameof(UserSettings.IsEnabled)), Expression.Constant(true));
            exp = GetGenresFilter(exp, video, param);
            exp = GetVideoTypeFilter(exp, video, param);
            exp = GetVotesCountFilter(exp, video, param);
            exp = GetVotesAverageFilter(exp, video, param);
            return db.Users.Where(Expression.Lambda<Func<User, bool>>(exp, param)).ToArray();
        }


        /// <summary>
        /// Применяет фильтр по жанрам.
        /// </summary>
        /// <param name="current">Текущее дерево выражений, к которому необходимо применить фильтр по жанрам.</param>
        /// <param name="video">Видео, по жанрам которого необходимо профильтровать пользователей.</param>
        /// <param name="parameter">Параметр выражения Linq, используемый при построении дерева.</param>
        /// <returns>Выражение с примененным фильтром.</returns>
        private static Expression GetGenresFilter(Expression current, Video video, ParameterExpression parameter)
        {
            if (video.Genres == null || !video.Genres.Any())
                return current;
            ulong[] genresIds = video.Genres.Select(x => x.GenreId).ToArray();
            Expression aggregationExpression = Expression.Not(Expression.Call(
                typeof(Enumerable),
                "Any",
                new[] { typeof(SettingGenre) },
                Expression.Property(Expression.Property(parameter, nameof(User.Settings)), nameof(UserSettings.MovieGenres))));

            foreach (ulong id in genresIds)
            {
                MethodCallExpression curCall = getGenresCallAnyExpression(id, parameter, video.Type);
                aggregationExpression = Expression.OrElse(aggregationExpression, curCall);
            }
            return Expression.AndAlso(aggregationExpression, current);
        }
        private static MethodCallExpression getGenresCallAnyExpression(
            ulong targetId,
            ParameterExpression parameter,
            VideoType type)
        {

            Expression<Func<SettingGenre, bool>> predicate =
                a => a.GenreId == targetId && (int)a.Genre.Type == (int)type;
            MethodCallExpression body = Expression.Call(
                typeof(Enumerable),
                "Any",
                new[] { typeof(SettingGenre) },
                Expression.Property(Expression.Property(parameter, nameof(User.Settings)), nameof(UserSettings.MovieGenres)),
                predicate);
            return body;
        }
        /// <summary>
        /// Применяет фильтр по типу видео.
        /// </summary>
        /// <param name="current">Текущее дерево выражений, к которому необходимо применить фильтр по типу видео.</param>
        /// <param name="video">Видео, по типу которого необходимо профильтровать пользователей.</param>
        /// <param name="parameter">Параметр выражения Linq, используемый при построении дерева.</param>
        /// <returns>Выражение с примененным фильтром.</returns>
        private static Expression GetVideoTypeFilter(Expression current, Video video, ParameterExpression parameter)
        {
            Expression exp;
            exp = Expression.OrElse(
                Expression.Equal(Expression.Property(
                Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.FilteringVideoType)),Expression.Constant(null)),
                Expression.Equal(Expression.Convert(
                        Expression.Property
                        (Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.FilteringVideoType)), typeof(int)),Expression.Constant((int)video.Type))

                            );
            return Expression.AndAlso(current, exp);
        }
        /// <summary>
        /// Применяет фильтр по количеству голосов рейтинга видео.
        /// </summary>
        /// <param name="current">Текущее дерево выражений, к которому необходимо применить фильтр по количеству голосов.</param>
        /// <param name="video">Видео, по числу голосов которого необходимо профильтровать пользователей.</param>
        /// <param name="parameter">Параметр выражения Linq, используемый при построении дерева.</param>
        /// <returns>Выражение с примененным фильтром.</returns>
        private static Expression GetVotesCountFilter(Expression current, Video video, ParameterExpression parameter)
        {
            Expression exp;
            exp = Expression.OrElse(
                Expression.Equal(Expression.Property(
                Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.VotesCount)), Expression.Constant(null)),
                Expression.LessThanOrEqual(
                        Expression.Property
                        (Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.VotesCount)),Expression.Convert(Expression.Constant(video.VotesCount),typeof(int?)))
                            );
            return Expression.AndAlso(current, exp);
        }

        /// <summary>
        /// Применяет фильтр по количеству голосов рейтинга видео.
        /// </summary>
        /// <param name="current">Текущее дерево выражений, к которому необходимо применить фильтр по количеству голосов.</param>
        /// <param name="video">Видео, по числу голосов которого необходимо профильтровать пользователей.</param>
        /// <param name="parameter">Параметр выражения Linq, используемый при построении дерева.</param>
        /// <returns>Выражение с примененным фильтром.</returns>
        private static Expression GetVotesAverageFilter(Expression current, Video video, ParameterExpression parameter)
        {
            Expression exp;
            exp = Expression.OrElse(
                Expression.Equal(Expression.Property(
                Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.VotesAverage)), Expression.Constant(null)),
                Expression.LessThanOrEqual(
                        Expression.Property
                        (Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.VotesAverage)),Expression.Convert(Expression.Constant(video.VotesAverage),typeof(float?)))
                            );
            return Expression.AndAlso(current, exp);
        }

    }
}
