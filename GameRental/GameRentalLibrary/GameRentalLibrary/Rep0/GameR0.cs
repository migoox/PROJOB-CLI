using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GameRental.Extensions;
using GameRental;
using GameRental.Collections;

namespace GameRental.Rep0
{
    public class GameRep0 : AbstractDatabaseEntity, IGame
    {
        public string Name { get; set; }

        public string Genre { get; set; }

        public string Devices { get; set; }

        public SyncList<IReview> Reviews { get; set; } = new SyncList<IReview>();

        public SyncList<IMod> Mods { get; set; } = new SyncList<IMod>();

        public SyncList<IUser> Authors { get; set; } = new SyncList<IUser>();

        public GameRep0(string name, string genre, string devices,
            List<IUser>? authors = null,
            List<IMod>? mods = null,
            List<IReview>? reviews = null) 
        {
            this.Name = name;
            this.Genre = genre;
            this.Devices = devices;

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

            if (authors != null)
                this.Authors = new SyncList<IUser>(authors);

            if (mods != null)
                this.Mods = new SyncList<IMod>(mods);

            if (reviews != null)
                this.Reviews = new SyncList<IReview>(reviews);
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
    }
}
