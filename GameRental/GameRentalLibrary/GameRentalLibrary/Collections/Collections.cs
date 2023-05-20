using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GameRental.Collections
{
    interface ICollection<T>
    {


    }

    interface IIterator<T>
    {
        public IIterator<T> Next();
        public T Value();
        public bool CompareWith(IIterator<T> it);
    }

    interface IIterableCollection<T> : ICollection<T>
    {
        public IIterator<T> Begin();
        public IIterator<T> End();
    }





}
