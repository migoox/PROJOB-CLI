using GameRental.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep0
{
    public class ModRep0 : AbstractDatabaseEntity, IMod
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public SyncList<IUser> Authors { get; set; } = new SyncList<IUser>();

        public SyncList<IMod> Compatibility { get; set; } = new SyncList<IMod>();

        public ModRep0(string name, string description, 
            List<IUser>? authors = null, 
            List<IMod>? mods = null)
        {
            this.Name = name;
            this.Description = description;

            Setters = new Dictionary<string, Action<object>>
            {
                ["name"] = x => this.Name = (string)x,
                ["description"] = x => this.Description = (string)x,
                ["authors"] = x => this.Authors = Database.Instance.GetByIds<IUser>((int[])x),
                ["compatibility"] = x => this.Compatibility = Database.Instance.GetByIds<IMod>((int[])x)
            };

            Getters = new Dictionary<string, Func<object>>
            {
                ["id"] = () => this.Id,
                ["name"] = () => this.Name,
                ["description"] = () => this.Description,
                ["authors"] = () => Database.Instance.GetByRefs(this.Authors),
                ["compatibility"] = () => Database.Instance.GetByRefs(this.Compatibility)
            };

            TypeName = "mod";

            if (authors != null)
                this.Authors = new SyncList<IUser>(authors);
            if (mods != null)
                this.Compatibility = new SyncList<IMod>(mods);
        }
        public override string ToString()
        {
            return $"[ID: {Id}] " + 
                   $"{Name}, " +
                   $"{Description}, " +
                   $"[{string.Join(", ", Authors.GetSynced().Select(u => u.Nickname))}], " +
                   $"[{string.Join(", ", Compatibility.GetSynced().Select(m => m.Id))}]";
        }
    }
}
