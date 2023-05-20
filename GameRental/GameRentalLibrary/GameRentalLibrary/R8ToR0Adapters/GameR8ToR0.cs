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
    public class GameR8ToR0 : AbstractDatabaseEntity, IGame, IDatabaseAdapter
    {
        private GameRep8 game;

        public GameR8ToR0(GameRep8 game)
        {
            this.game = game;
            Setters = new Dictionary<string, Action<object>>
            {
                ["name"] = x => this.Name = (string)x,
                ["genre"] = x => this.Genre = (string)x,
                ["devices"] = x => this.Devices = (string)x,
                ["reviews"] = x => this.Reviews = Database.Instance.GetByIds<IReview>((int[])x),
                ["mods"] = x => this.Mods = Database.Instance.GetByIds<IMod>((int[])x),
                ["authors"] = x => this.Authors = Database.Instance.GetByIds<IUser>((int[])x)
            };

            Getters = new Dictionary<string, Func<object>>
            {
                ["id"] = () => this.Id,
                ["name"] = () => this.Name,
                ["genre"] = () => this.Genre,
                ["devices"] = () => this.Devices,
                ["reviews"] = () => Database.Instance.GetByRefs(this.Reviews),
                ["mods"] = () => Database.Instance.GetByRefs(this.Mods),
                ["authors"] = () => Database.Instance.GetByRefs(this.Authors)
            };

            TypeName = "game";
        }
        public void Adapt(IStackRepresentation rep)
        {
            this.game = (GameRep8)rep;
        }

        public override string ToString()
        {
            return $"[ID: {Id}] " +
                   $"{Name}, " +
                   $"{Genre}, " +
                   $"[{string.Join(", ", Authors.GetSynced().Select(u => u.Id))}]," +
                   $"[{string.Join(", ", Reviews.GetSynced().Select(r => r.Id))}]," +
                   $"[{string.Join(", ", Mods.GetSynced().Select(m => m.Id))}]," +
                   $"{Devices}";
        }

        public string Name
        {
            get
            {
                return string.Join("", game.GetVariableFromStack("name")); 
            }
            set
            {
                game.SetVariableOnStack("name", new string[]{ value });
            }
        }

        public string Genre
        {
            get
            {
                return string.Join("", game.GetVariableFromStack("genre"));
            }
            set
            {
                game.SetVariableOnStack("genre", new string[] { value });
            }
        }

        public string Devices
        {
            get
            {
                return string.Join("", game.GetVariableFromStack("devices"));
            }
            set
            {
                game.SetVariableOnStack("devices", new string[] { value });
            }
        }
        public SyncList<IReview> Reviews
        {
            get
            {
                return new SyncList<IReview>(game.GetCollectionFromStack<IReview>("reviews"));
            }
            set
            {
                game.SetCollectionOnStack("reviews", value.GetSynced());
            }
        }

        public SyncList<IMod> Mods
        {
            get
            {
                return new SyncList<IMod>(game.GetCollectionFromStack<IMod>("mods"));
            }
            set
            {
                game.SetCollectionOnStack("mods", value.GetSynced());
            }
        }

        public SyncList<IUser> Authors
        {
            get
            {
                return new SyncList<IUser>(game.GetCollectionFromStack<IUser>("authors"));
            }
            set
            {
                game.SetCollectionOnStack("authors", value.GetSynced());
            }
        }
    }
}
