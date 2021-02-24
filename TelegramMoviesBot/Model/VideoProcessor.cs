using System;
using System.Linq;
using System.Linq.Expressions;
using MoviesDatabase;
using MoviesDatabase.DatabaseModel;
using MoviesDatabase.DatabaseModel.ManyToManyTables;
using TelegramMoviesBot.Model.VideoDataProviders;

namespace TelegramMoviesBot.Model
{
    public class VideoProcessor
    {
        private IVideoDataProvider VideoProvider;

        public VideoProcessor(IVideoDataProvider videoProvider)
        {
            VideoProvider = videoProvider;
        }


        public void Start()
        {
            try
            {
                Video[] newVideos = VideoProvider.GetNewVideos();
                DatabaseContext databaseContext = new DatabaseContext();
                string[] curNames = databaseContext.Videos.Select(x => x.Name).ToArray();
                newVideos = newVideos.Where(x => !curNames.Contains(x.Name?.ToLower())).ToArray();
                curNames = null;
                foreach (Video video in newVideos)
                {
                    User[] usersWithNotifications = CheckUsers(databaseContext, video);
                    foreach (User notifyUser in usersWithNotifications)
                    {
                        FakeSendMessage(notifyUser, video);
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
        }


        private static User[] CheckUsers(DatabaseContext db, Video video)
        {
            ParameterExpression param = Expression.Parameter(typeof(User), "x");
            Expression exp = Expression.Equal(
    Expression.Property(
        Expression.Property(param, nameof(User.Settings)), nameof(UserSettings.IsEnabled)), Expression.Constant(true));
            exp = GetGenresFilter(exp, video, param);
            exp = GetCountriesFilter(exp, video, param);
            return db.Users.Where(Expression.Lambda<Func<User, bool>>(exp, param)).ToArray();
        }



        private static Expression GetGenresFilter(Expression current, Video video, ParameterExpression parameter)
        {
            if (video.Genres == null || !video.Genres.Any())
                return current;
            ulong[] genresIds = video.Genres.Select(x => x.GenreId).ToArray();
            Expression aggregationExpression = getGenresCallAnyExpression(genresIds.First(), parameter);
            foreach (ulong id in genresIds.Skip(1))
            {
                MethodCallExpression curCall = getGenresCallAnyExpression(id, parameter);
                aggregationExpression = Expression.OrElse(aggregationExpression, curCall);
            }
            return Expression.AndAlso(aggregationExpression, current);
        }
        private static MethodCallExpression getGenresCallAnyExpression(ulong targetId, ParameterExpression parameter)
        {

            Expression<Func<SettingGenre, bool>> predicate =
                a => a.GenreId == targetId;
            MethodCallExpression body = Expression.Call(
                typeof(Enumerable),
                "Any",
                new[] { typeof(SettingGenre) },
                Expression.Property(Expression.Property(parameter,nameof(User.Settings)), nameof(UserSettings.Genres)),
                predicate);
            return body;
        }

        private static Expression getVideoTypeFilter(Expression current, Video video, ParameterExpression parameter)
        {
            Expression<Func<User, bool>> exp = null;
            //Отвратительная реализация от того, что пока что впадлу по другому сделать.
            if (video.Type == 0 || (video.Type.HasFlag(VideoType.Movie) && video.Type.HasFlag(VideoType.TvSeries)))
                return current;
            if (video.Type.HasFlag(VideoType.Movie))
                exp = user => (user.Settings.FilteringVideoType & VideoType.Movie) != 0;
            else
                exp = user => (user.Settings.FilteringVideoType & VideoType.TvSeries) != 0;
            return Expression.AndAlso(current, exp);
        }


        private static Expression GetCountriesFilter(Expression current, Video video, ParameterExpression parameter)
        {
            if (video.Countries == null || !video.Countries.Any())
                return current;
            ulong[] countriesIds = video.Countries.Select(x => x.CountryId).ToArray();

            Expression aggregationExpression = getCountriesCallAnyExpression(countriesIds.First(), parameter);
            foreach (ulong id in countriesIds.Skip(1))
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

        

        private static void FakeSendMessage(User user, Video video) { }


    }
}
