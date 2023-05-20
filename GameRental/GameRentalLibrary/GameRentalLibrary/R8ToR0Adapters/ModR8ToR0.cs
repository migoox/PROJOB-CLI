using GameRental.Rep8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GameRental.Collections;
using GameRental.Rep0;
using GameRental.Extensions;

namespace GameRental.Adapters
{
    public class ModR8ToR0 : AbstractDatabaseEntity, IMod, IDatabaseAdapter
    {
        private ModRep8 mod;

        public string Name
        {
            get
            {
                return string.Join("", mod.GetVariableFromStack("name"));
            }
            set
            {
                mod.SetVariableOnStack("name", new string[] { value });
            }
        }

        public string Description
        {
            get
            {
                return string.Join("", mod.GetVariableFromStack("description"));
            }
            set
            {
                mod.SetVariableOnStack("description", new string[] { value });
            }
        }

        public SyncList<IUser> Authors
        {
            get
            {
                return new SyncList<IUser>(mod.GetCollectionFromStack<IUser>("authors"));
            }
            set
            {
                mod.SetCollectionOnStack("authors", value.GetSynced());
            }
        }

        public SyncList<IMod> Compatibility
        {
            get
            {
                return new SyncList<IMod>(mod.GetCollectionFromStack<IMod>("compatibility"));
            }
            set
            {
                mod.SetCollectionOnStack("compatibility", value.GetSynced());
            }
        }

        public ModR8ToR0(ModRep8 mod)
        {
            this.mod = mod;

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
        }
        public void Adapt(IStackRepresentation rep)
        {
            this.mod = (ModRep8)rep;
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
