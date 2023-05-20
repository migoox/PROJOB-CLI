using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep8
{

    public class ModRep8 : IStackRepresentation
    {
        private static int currentObjectId = 1;

        private Tuple<int, Stack<string>> data;

        public ModRep8(
            string name,
            string description,
            int[] authorsIds,
            int[] compabilityIds)
        {
            data = new Tuple<int, Stack<string>>
                (currentObjectId++, new Stack<string>());

            // name
            data.Item2.Push(name);
            data.Item2.Push("1");
            data.Item2.Push("name");

            // description
            data.Item2.Push(description);
            data.Item2.Push("1");
            data.Item2.Push("description");

            // authorsIds
            foreach (var elem in authorsIds)
                Data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(authorsIds.Length));
            data.Item2.Push("authors");

            // compabilityIds
            foreach (var elem in compabilityIds)
                Data.Item2.Push(Convert.ToString(elem));
            data.Item2.Push(Convert.ToString(compabilityIds.Length));
            data.Item2.Push("compatibility");
        }

        public Tuple<int, Stack<string>> Data
        {
            get => data;
            set => data = value;
        }
    }
}
