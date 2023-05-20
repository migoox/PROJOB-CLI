using GameRental.Rep0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Collections;

namespace GameRental.Extensions
{
    public static class GameDatabaseExtension
    {
        public static void SetAuthorsByIds(this IGame game, int[] ids)
        {
            game.Authors = new SyncList<IUser>(new List<IUser>(
                Database.Instance.Users.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }

        public static void SetModsByIds(this IGame game, int[] ids)
        {
            game.Mods = new SyncList<IMod>(new List<IMod>(
                Database.Instance.Mods.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }
        public static void SetReviewsByIds(this IGame game, int[] ids)
        {
            game.Reviews = new SyncList<IReview>(new List<IReview>(
                Database.Instance.Reviews.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }
    }

    public static class ModDatabaseExtension
    {
        public static void SetAuthorsByIds(this IMod mod, int[] ids)
        {
            mod.Authors = new SyncList<IUser>(new List<IUser>(
                Database.Instance.Users.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }

        public static void SetModsByIds(this IMod mod, int[] ids)
        {
            mod.Compatibility = new SyncList<IMod>(new List<IMod>(
                Database.Instance.Mods.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }
    }

    public static class ReviewDatabaseExtension
    {

        public static void SetAuthorById(this IReview review, int id)
        {
            IUser? result = Database.Instance.Users.Values.FirstOrDefault(g => id == g.Id);
            if (result != null)
            {
                review.Author = result;
            }
        }
    }
    public static class UserDatabaseExtension
    {
        public static void SetGamesByIds(this IUser user, int[] ids)
        {
            user.OwnedGames = new SyncList<IGame>(new List<IGame>(
                Database.Instance.Games.
                    Select(g => g.Value).
                    Where(g => ids.Contains(g.Id))));
        }
    }

}
