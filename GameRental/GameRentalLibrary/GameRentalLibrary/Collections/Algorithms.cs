using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Collections
{
    class Algorithms
    {
        public static T FindIf<T>(Deque<T> list, Func<T, bool> predicate)
        {
            var it = list.Begin();
            while (!it.CompareWith(list.End()))
            {
                if (predicate(it.Value()))
                {
                    return it.Value();
                }
                it = it.Next();
            }
            return default(T);
        }

        public static void ForEach<T>(Deque<T> list, Action<T> action)
        {
            var it = list.Begin();
            while (!it.CompareWith(list.End()))
            {
                action(it.Value());
                it = it.Next();
            }
        }

        public static int CountIf<T>(Deque<T> list, Func<T, bool> predicate)
        {
            int count = 0;
            var it = list.Begin();
            while (!it.CompareWith(list.End()))
            {
                if (predicate(it.Value()))
                {
                    count++;
                }
                it = it.Next();
            }
            return count;
        }
    }
}
