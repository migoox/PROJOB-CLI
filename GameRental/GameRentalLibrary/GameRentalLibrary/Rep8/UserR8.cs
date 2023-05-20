using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep8
{
    public class UserRep8 : IStackRepresentation
    {
        private static int currentObjectId = 1;
        private Tuple<int, Stack<string>> data;

        public UserRep8(
            string nickname,
            int[] ownedGamesIds)
        {
            data = new Tuple<int, Stack<string>>
                (currentObjectId++, new Stack<string>());

            // nickname
            data.Item2.Push(nickname);
            data.Item2.Push("1");
            data.Item2.Push("nickname");

            // ownedGamesIds
            foreach (var elem in ownedGamesIds)
                Data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(ownedGamesIds.Length));
            data.Item2.Push("games");
        }

        public Tuple<int, Stack<string>> Data
        {
            get => data;
            set => data = value;
        }
    }
}
