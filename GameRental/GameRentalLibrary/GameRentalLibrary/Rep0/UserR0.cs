using GameRental.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep0
{
    public class UserRep0 : AbstractDatabaseEntity, IUser
    {
        public string Nickname { get; set; }

        public SyncList<IGame> OwnedGames { get; set; } = new SyncList<IGame>();

        public UserRep0(string nickname, List<IGame>? games = null)
        {
            this.Nickname = nickname;

            Setters = new Dictionary<string, Action<object>>
            {
                ["nickname"] = x => this.Nickname = (string)x,
                ["owned"] = x => this.OwnedGames = Database.Instance.GetByIds<IGame>((int[])x)
            };

            Getters = new Dictionary<string, Func<object>>
            {
                ["id"] = () => this.Id,
                ["nickanme"] = () => this.Nickname,
                ["owned"] = () => Database.Instance.GetByRefs(this.OwnedGames)
            };

            TypeName = "user";

            if (games != null)
                this.OwnedGames = new SyncList<IGame>(games);
        }

        public override string ToString()
        {
            return $"[ID: {Id}] " +
                   $"{Nickname}," +
                   $" [{string.Join(", ", OwnedGames.GetSynced().Select(g => g.Id))}]";
        }
    }
}
