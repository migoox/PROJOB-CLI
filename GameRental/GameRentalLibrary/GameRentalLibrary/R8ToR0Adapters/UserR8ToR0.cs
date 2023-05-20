using GameRental.Rep8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Collections;
using GameRental.Rep0;
using GameRental.Extensions;

namespace GameRental.Adapters
{
    public class UserR8ToR0 : AbstractDatabaseEntity, IUser, IDatabaseAdapter
    {
        private UserRep8 user;

        public string Nickname
        {
            get
            {
                return string.Join("", user.GetVariableFromStack("nickname"));
            }
            set
            {
                user.SetVariableOnStack("nickname", new string[]{value});
            }
        }

        public SyncList<IGame> OwnedGames
        {
            get
            {
                return new SyncList<IGame>(user.GetCollectionFromStack<IGame>("games"));
            }
            set
            {
                user.SetCollectionOnStack("games", value.GetSynced());
            }
        }
        public UserR8ToR0(UserRep8 user)
        {
            this.user = user;
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
        }

        public void Adapt(IStackRepresentation rep)
        {
            this.user = (UserRep8)rep;
        }
        public override string ToString()
        {
            return $"[ID: {Id}] " +
                   $"{Nickname}," +
                   $" [{string.Join(", ", OwnedGames.GetSynced().Select(g => g.Id))}]";
        }
    }
}
