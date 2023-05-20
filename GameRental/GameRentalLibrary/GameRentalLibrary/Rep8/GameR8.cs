using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep8
{
    public class GameRep8 : IStackRepresentation
    {
        private static int currentObjectId = 1;
        private Tuple<int, Stack<string>> data;

        public GameRep8(
            string name,
            string genre,
            int[] authorsIds,
            int[] reviewsIds,
            int[] modsIds,
            string devices)
        {
            data = new Tuple<int, Stack<string>>
                (currentObjectId++, new Stack<string>());

            // name
            data.Item2.Push(name);
            data.Item2.Push("1");
            data.Item2.Push("name");

            // genre
            data.Item2.Push(genre);
            data.Item2.Push("1");
            data.Item2.Push("genre");

            // authorsIds
            foreach (var elem in authorsIds)
                Data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(authorsIds.Length));
            data.Item2.Push("authors");

            // reviewsIds
            foreach (var elem in reviewsIds)
                Data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(reviewsIds.Length));
            data.Item2.Push("reviews");

            // modsIds
            foreach (var elem in modsIds)
                data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(modsIds.Length));
            data.Item2.Push("mods");

            // devices
            data.Item2.Push(devices);
            data.Item2.Push("1");
            data.Item2.Push("devices");
        }

        public Tuple<int, Stack<string>> Data
        {
            get => data;
            set => data = value;
        }
    }
}
