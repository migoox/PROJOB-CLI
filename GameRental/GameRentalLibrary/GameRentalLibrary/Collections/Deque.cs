using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Collections
{
    class DequeNode<T>
    {
        static public int MaxElements = 2;
        public DequeNode<T>? prev;
        public DequeNode<T>? next;
        public List<T> vector;

        public DequeNode()
        {
            vector = new List<T>(MaxElements);
            prev = null;
            next = null;
        }

        public DequeNode(DequeNode<T>? prev, DequeNode<T>? next)
        {
            vector = new List<T>(MaxElements);
            this.prev = prev;
            this.next = next;
        }
    }

    class DequeIterator<T> : IIterator<T>
    {
        private DequeNode<T>? _node;
        private int _currIndex;

        public DequeIterator(DequeNode<T>? node, int currIndex)
        {
            _node = node;
            _currIndex = currIndex;
        }
        public IIterator<T> Next()
        {
            if (_currIndex == DequeNode<T>.MaxElements - 1)
                return new DequeIterator<T>(_node.next, 0);

            return new DequeIterator<T>(_node, _currIndex + 1);
        }

        public T Value()
        {
            return _node.vector[_currIndex];
        }

        public bool CompareWith(IIterator<T> it)
        {
            if (it is not DequeIterator<T>)
                return false;

            var _it = it as DequeIterator<T>;
            return this._node == _it._node && this._currIndex == _it._currIndex;
        }
    }
    class ReversedDequeIterator<T> : IIterator<T>
    {
        private DequeNode<T>? _node;
        private int _currIndex;

        public ReversedDequeIterator(DequeNode<T>? node, int currIndex)
        {
            _node = node;
            _currIndex = currIndex;
        }
        public IIterator<T> Next()
        {
            if (_currIndex == 0)
            {
                if (_node.prev == null)
                    return new ReversedDequeIterator<T>(null, 0);
                else
                    return new ReversedDequeIterator<T>(_node.prev, _node.prev.vector.Count - 1);
            }

            return new ReversedDequeIterator<T>(_node, _currIndex - 1);
        }

        public T Value()
        {
            return _node.vector[_currIndex];
        }

        public bool CompareWith(IIterator<T> it)
        {
            if (it is not ReversedDequeIterator<T>)
                return false;

            var _it = it as ReversedDequeIterator<T>;
            return this._node == _it._node && this._currIndex == _it._currIndex;
        }
    }
    class Deque<T> : IIterableCollection<T>
    {
        private DequeNode<T>? _head;
        private DequeNode<T>? _tail;
        public Deque()
        {
            _head = null;
            _tail = null;
        }

        public void PushBack(T element)
        {
            if (_head == null)
            {
                _head = new DequeNode<T>(null, null);
                _tail = _head;
                _tail.vector.Add(element);
                return;
            }

            if (_tail.vector.Count == DequeNode<T>.MaxElements)
            {
                var newTail = new DequeNode<T>(_tail, null);
                _tail.next = newTail;
                _tail = newTail;
            }

            _tail.vector.Add(element);
        }
        public void PushFront(T element)
        {
            if (_head == null)
            {
                _head = new DequeNode<T>(null, null);
                _tail = _head;
                _tail.vector.Add(element);
                return;
            }

            if (_head.vector.Count == DequeNode<T>.MaxElements)
            {
                var newHead = new DequeNode<T>(null, _head);
                _head.prev = newHead;
                _head = newHead;
            }
            else
            {
                _tail.vector.Insert(0, element);
            }
        }
        public void PopBack()
        {
            // TO DO
            _tail.vector.RemoveAt(_tail.vector.Count - 1);
        }
        public void PopFront()
        {
            // TO DO
            _head.vector.RemoveAt(0);
        }

        public T Front()
        {
            return _head.vector[0];
        }

        public T Back()
        {
            return _tail.vector[_tail.vector.Count - 1];
        }

        public IIterator<T> RBegin()
        {
            return new ReversedDequeIterator<T>(_tail, _tail.vector.Count - 1);
        }

        public IIterator<T> REnd()
        {
            return new ReversedDequeIterator<T>(null, 0);
        }

        public IIterator<T> Begin()
        {
            return new DequeIterator<T>(_head, 0);
        }

        public IIterator<T> End()
        {
            if (_tail.vector.Count == DequeNode<T>.MaxElements)
                return new DequeIterator<T>(null, 0);
            return new DequeIterator<T>(_tail, _tail.vector.Count);
        }
    }
}
