using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep8
{
    public class ReviewRep8 : IStackRepresentation
    {
        private static int currentObjectId = 1;
        private Tuple<int, Stack<string>> data;

        public ReviewRep8(
            string text,
            int rating,
            int authorId)
        {
            data = new Tuple<int, Stack<string>>
                (currentObjectId++, new Stack<string>());

            // text
            data.Item2.Push(text);
            data.Item2.Push("1");
            data.Item2.Push("text");

            // rating
            data.Item2.Push(Convert.ToString(rating));
            data.Item2.Push("1");
            data.Item2.Push("rating");

            // authorId
            data.Item2.Push(Convert.ToString(authorId));
            data.Item2.Push("1");
            data.Item2.Push("author");
        }

        public Tuple<int, Stack<string>> Data
        {
            get => data;
            set => data = value;
        }
    }
    
}
