using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Collections
{
    internal class VectorIterator<T> : IIterator<T>
    {
        private T[] _arr;
        int _index;
        public VectorIterator(T[] arr, int index)
        {
            _arr = arr;
            _index = index;
        }
        public IIterator<T> Next()
        {
            return new VectorIterator<T>(_arr, _index + 1);
        }

        public T Value()
        {
            return _arr[_index];
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is VectorIterator<T>)) return false;

            var o = obj as VectorIterator<T>;

            return _arr == o._arr && _index == o._index;
        }

        public static bool operator ==(VectorIterator<T> it1, VectorIterator<T> it2)
        {
            return it1.Equals(it2);
        }
        public static bool operator !=(VectorIterator<T> it1, VectorIterator<T> it2)
        {
            return !it1.Equals(it2);
        }

        public bool CompareWith(IIterator<T> it)
        {
            return false;
        }
    }

    internal class ReversedVectorIterator<T> : IIterator<T>
    {
        private T[] _arr;
        int _index;
        public ReversedVectorIterator(T[] arr, int index)
        {
            _arr = arr;
            _index = index;
        }
        public IIterator<T> Next()
        {
            return new ReversedVectorIterator<T>(_arr, _index - 1);
        }

        public T Value()
        {
            return _arr[_index];
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is ReversedVectorIterator<T>)) return false;

            var o = obj as ReversedVectorIterator<T>;

            return _arr == o._arr && _index == o._index;
        }

        public static bool operator ==(ReversedVectorIterator<T> it1, ReversedVectorIterator<T> it2)
        {
            return it1.Equals(it2);
        }
        public static bool operator !=(ReversedVectorIterator<T> it1, ReversedVectorIterator<T> it2)
        {
            return !it1.Equals(it2);
        }

        public bool CompareWith(IIterator<T> it)
        {
            return false;
        }
    }

    internal class Vector<T> : IIterableCollection<T>
    {
        private T[] _array;
        private int _realCount;
        public Vector()
        {
            _array = new T[4];
            _realCount = 0;
        }
        public Vector(params T[] list)
        {
            _array = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
                _array[i] = list[i];
            _realCount = list.Length;
        }

        public void PushBack(T item)
        {
            if (_array.Length < _realCount + 1)
            {
                T[] _array2 = new T[2 * _array.Length];
                for (int i = 0; i < _array.Length; i++)
                    _array2[i] = _array[i];
                _array = _array2;
            }
            _array[_realCount++] = item;
        }

        public void PopBack()
        {
            --_realCount;
        }

        public T Front()
        {
            return _array[0];
        }
        public T Back()
        {
            return _array[_realCount - 1];
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < _realCount; ++i)
            {
                builder.Append(_array[i]);
                builder.Append(", ");
            }
            if (builder.Length > 0)
                builder.Remove(builder.Length - 2, 1);

            return builder.ToString();
        }

        public void PrintDebug()
        {
            Console.WriteLine("Vector -- Debug Info:");
            Console.WriteLine($"Capacity: {_array.Length}");
            Console.WriteLine($"Size: {_realCount}");
            Console.WriteLine($"Elements:\n{this}\n\n");


        }
        public IIterator<T> Begin()
        {
            return new VectorIterator<T>(_array, 0);
        }

        public IIterator<T> End()
        {
            return new VectorIterator<T>(_array, _realCount);
        }

        public IIterator<T> RBegin()
        {
            return new ReversedVectorIterator<T>(_array, _realCount - 1);
        }

        public IIterator<T> REnd()
        {
            return new ReversedVectorIterator<T>(_array, -1);
        }
    }
}
