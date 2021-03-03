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
            

        }

        

        public void Start()
        {
            while (true)
            {
                Thread.Sleep(60 * UpdateIntervalInMinutes);
                PerformUpdate();
            }

#if DEBUG
            //PerformUpdate(null, null);
#endif
        }

        

        private async void PerformUpdate()
        {
            await Task.Run(() =>
            {
                try
                {





                    Video[] newVideos = VideoProvider.GetNewVideos().ToArray();
                    DatabaseContext databaseContext = new DatabaseContext();
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
                        databaseContext.Videos.Add(video);
                    }
                    databaseContext.SaveChanges();
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
            exp = GetCountriesFilter(exp, video, param);
            exp = getVideoTypeFilter(exp, video, param);
            return db.Users.Where(Expression.Lambda<Func<User, bool>>(exp, param)).ToArray();
        }



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

        private static Expression getVideoTypeFilter(Expression current, Video video, ParameterExpression parameter)
        {
            Expression exp;
            //Отвратительная реализация от того, что пока что впадлу по другому сделать.
            if (video.Type == 0 || (video.Type.HasFlag(VideoType.Movie) && video.Type.HasFlag(VideoType.TvSeries)))
                return current;
            if (video.Type.HasFlag(VideoType.Movie))
                exp = Expression.NotEqual(
                    Expression.And(
                        Expression.Convert(
                        Expression.Property
                        (Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.FilteringVideoType)), typeof(int)),
                        Expression.Constant((int)VideoType.Movie)), Expression.Constant(0));
            else
                exp = Expression.NotEqual(
                    Expression.And(
                        Expression.Convert(
                        Expression.Property
                        (Expression.Property(
                            parameter, nameof(User.Settings)), nameof(UserSettings.FilteringVideoType)), typeof(int)),
                        Expression.Constant((int)VideoType.TvSeries)), Expression.Constant(0));
            return Expression.AndAlso(current, exp);
        }




        private static Expression GetCountriesFilter(Expression current, Video video, ParameterExpression parameter)
        {
            if (video.Countries == null || !video.Countries.Any())
                return current;
            ulong[] countriesIds = video.Countries.Select(x => x.CountryId).ToArray();

            Expression aggregationExpression = Expression.Not(Expression.Call(
                typeof(Enumerable),
                "Any",
                new[] { typeof(SettingCountry) },
                Expression.Property(Expression.Property(parameter, nameof(User.Settings)), nameof(UserSettings.Countries))));
            foreach (ulong id in countriesIds)
            {
                MethodCallExpression curCall = getCountriesCallAnyExpression(id, parameter);
                aggregationExpression = Expression.OrElse(aggregationExpression, curCall);
            }
            return Expression.AndAlso(aggregationExpression, current);
        }

        private static MethodCallExpression getCountriesCallAnyExpression(ulong targetId, ParameterExpression parameter)
        {
            Expression<Func<SettingCountry, bool>> predicate =
                a => a.CountryId == targetId;
            MethodCallExpression body = Expression.Call(
                typeof(Enumerable),
                "Any",
                new[] { typeof(SettingCountry) },
                Expression.Property(Expression.Property(parameter, nameof(User.Settings)), nameof(UserSettings.Countries)),
                predicate);
            return body;
        }

    }
}
