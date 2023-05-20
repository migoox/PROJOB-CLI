using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace GameRental.Collections
{
    internal class DListNode<T>
    {
        public T value;
        public DListNode<T>? prev;
        public DListNode<T>? next;
        public DListNode(T value)
        {
            this.value = value;
            prev = null;
            next = null;
        }

        public DListNode(T value, DListNode<T>? prev, DListNode<T>? next)
        {
            this.value = value;
            this.prev = prev;
            this.next = next;
        }
    }

    class DListIterator<T> : IIterator<T>
    {
        private DListNode<T>? _node;

        public DListIterator(DListNode<T>? node)
        {
            _node = node;
        }
        public IIterator<T> Next()
        {
            return new DListIterator<T>(_node.next);
        }

        public T Value()
        {
            return _node.value;
        }

        public bool CompareWith(IIterator<T> it)
        {
            if (it is not DListIterator<T>)
                return false;

            var _it = it as DListIterator<T>;
            return this._node == _it._node;
        }
    }
    class ReversedDListIterator<T> : IIterator<T>
    {
        private DListNode<T>? _node;

        public ReversedDListIterator(DListNode<T>? node)
        {
            _node = node;
        }
        public IIterator<T> Next()
        {
            return new ReversedDListIterator<T>(_node.prev);
        }

        public T Value()
        {
            return _node.value;
        }

        public bool CompareWith(IIterator<T> it)
        {
            if (it is not ReversedDListIterator<T>)
                return false;

            var _it = it as ReversedDListIterator<T>;
            return this._node == _it._node;
        }
    }

    internal class DList<T> : IIterableCollection<T>
    {
        private DListNode<T>? _head;
        private DListNode<T>? _tail;

        public DList()
        {
            _head = null;
            _tail = null;
        }
        public DList(params T[] initList)
        {
            foreach (var item in initList)
            {
                PushBack(item);
            }
        }

        void PushBack(T value)
        {
            if (_head == null)
            {
                _head = _tail = new DListNode<T>(value);
                return;
            }
            DListNode<T> newNode = new DListNode<T>(value, _tail, null);
            _tail.next = newNode;
            _tail = newNode;
        }

        void PushFront(T value)
        {
            if (_head == null)
            {
                _head = _tail = new DListNode<T>(value);
                return;
            }
            DListNode<T> newNode = new DListNode<T>(value, null, _head);
            _head.prev = newNode;
            _head = newNode;
        }

        void PopBack(T value)
        {
            if (_head == _tail)
            {
                _head = _tail = null;
                return;
            }

            _tail = _tail.prev;
            _tail.next = null;
        }

        void PopFront(T value)
        {
            if (_head == _tail)
            {
                _head = _tail = null;
                return;
            }

            _head = _head.next;
            _head.prev = null;
        }

        public IIterator<T> Begin()
        {
            return new DListIterator<T>(_head);
        }

        public IIterator<T> End()
        {
            return new DListIterator<T>(null);
        }

        public IIterator<T> RBegin()
        {
            return new ReversedDListIterator<T>(_tail);
        }

        public IIterator<T> REnd()
        {
            return new ReversedDListIterator<T>(null);
        }
    }
}
