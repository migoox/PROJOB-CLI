using GameRental.Rep0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Rep8;
using GameRental.Adapters;

namespace GameRental.Builders
{
    public class UserBuilder : AbstractBuilder, IDatabaseBuilder
    {
        private string nickname = "null";
        private List<IGame> games = new List<IGame>();

        public UserBuilder()
        {
            Setters = new Dictionary<string, Func<object, AbstractBuilder>>
            {
                ["nickname"] = x => WithNickname((string)x),
                ["owned"] = x => WithOwnedGamesByIds((int[])x)
            };

            Types = new Dictionary<string, Type>
            {
                ["nickname"] = typeof(string),
                ["owned"] = typeof(int[])
            };
        }

        public UserBuilder WithNickname(string nickname)
        {
            this.nickname = nickname;
            return this;
        }
        public UserBuilder WithOwnedGames(List<IGame> games)
        {
            this.games = games;
            return this;
        }
        public UserBuilder WithOwnedGamesByIds(int[] gamesIds)
        {
            this.games = Database.Instance.Games
                .Select(x => x.Value)
                .Where(x => gamesIds.Contains(x.Id))
                .ToList();
            return this;
        }

        public IDatabaseEntity BuildRep0()
        {
            return new UserRep0(nickname, games);
        }

        public IStackRepresentation BuildRep8()
        {
            return new UserRep8(nickname, games.Select(x => x.Id).ToArray());
        }

        public IDatabaseEntity BuildRep8AndAdapt()
        {
            return new UserR8ToR0((UserRep8)BuildRep8());
        }
        public override string ToString()
        {
            return nickname + ", " +
                   "[" + string.Join(", ", games.Select(x => x.Id).ToArray()) + "]";
        }
    }
}
