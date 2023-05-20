using GameRental.Rep0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using GameRental.Adapters;
using GameRental.Rep8;

namespace GameRental.Builders
{
    public class ModBuilder : AbstractBuilder, IDatabaseBuilder
    {
        private string name = "null";
        private string description = "null";
        private List<IUser> authors = new List<IUser>();
        private List<IMod> compatibility = new List<IMod>();

        public ModBuilder()
        {
            Setters = new Dictionary<string, Func<object, AbstractBuilder>>
            {
                ["name"] = x => WithName((string)x),
                ["description"] = x => WithDescription((string)x),
                ["authors"] = x => WithAuthorsByIds((int[])x),
                ["compatibility"] = x => WithCompatibilitiesByIds((int[])x)
            };

            Types = new Dictionary<string, Type>
            {
                ["name"] = typeof(string),
                ["description"] = typeof(string),
                ["authors"] = typeof(int[]),
                ["compatibility"] = typeof(int[]),
            };
        }

        public ModBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public ModBuilder WithDescription(string description)
        {
            this.description = description;
            return this;
        }

        public ModBuilder WithAuthors(List<IUser> authors)
        {
            this.authors = authors;
            return this;
        }
        public ModBuilder WithAuthorsByIds(int[] authorsIds)
        {
            this.authors = Database.Instance.Users
                .Select(x => x.Value)
                .Where(x => authorsIds.Contains(x.Id))
                .ToList();
            return this;
        }

        public ModBuilder WithCompatibilities(List<IMod> mods)
        {
            this.compatibility = mods;
            return this;
        }
        public ModBuilder WithCompatibilitiesByIds(int[] modsIds)
        {
            this.compatibility = Database.Instance.Mods
                .Select(x => x.Value)
                .Where(x => modsIds.Contains(x.Id))
                .ToList();
            return this;
        }

        public IDatabaseEntity BuildRep0()
        {
            return new ModRep0(name, description, authors, compatibility);
        }

        public IStackRepresentation BuildRep8()
        {
            return new ModRep8(name, description,
                authors.Select(x => x.Id).ToArray(),
                compatibility.Select(x => x.Id).ToArray());
        }

        public IDatabaseEntity BuildRep8AndAdapt()
        {
            return new ModR8ToR0((ModRep8)BuildRep8());
        }

        public override string ToString()
        {
            return name +
                   description + ", " +
                   "[" + string.Join(", ", authors.Select(x => x.Id).ToArray()) + "]" +
                   "[" + string.Join(", ", compatibility.Select(x => x.Id).ToArray()) + "]";
        }
    }
}
