using System;
using System.Collections.Generic;
using System.Threading;

namespace UniRx.InternalUtil {

    internal class PriorityQueue<T> where T : IComparable<T> {

        struct IndexedItem : IComparable<IndexedItem> {

            public T Value;
            public long Id;

            public int CompareTo(IndexedItem other) {
                int c = Value.CompareTo(other.Value);
                if (c == 0)
                    c = Id.CompareTo(other.Id);
                return c;
            }

        }

        private static long _count = long.MinValue;

        private IndexedItem[] _items;
        private int _size;

        public PriorityQueue() : this(16) { }

        public PriorityQueue(int capacity) {
            _items = new IndexedItem[capacity];
            _size = 0;
        }

        public int Count {
            get { return _size; }
        }

        private bool IsHigherPriority(int left, int right) { return _items[left].CompareTo(_items[right]) < 0; }

        private void Percolate(int index) {
            if (index >= _size || index < 0)
                return;
            int parent = (index - 1) / 2;
            if (parent < 0 || parent == index)
                return;

            if (IsHigherPriority(index, parent)) {
                IndexedItem temp = _items[index];
                _items[index] = _items[parent];
                _items[parent] = temp;
                Percolate(parent);
            }
        }

        private void Heapify() { Heapify(0); }

        private void Heapify(int index) {
            if (index >= _size || index < 0)
                return;

            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int first = index;

            if (left < _size && IsHigherPriority(left, first))
                first = left;
            if (right < _size && IsHigherPriority(right, first))
                first = right;
            if (first != index) {
                IndexedItem temp = _items[index];
                _items[index] = _items[first];
                _items[first] = temp;
                Heapify(first);
            }
        }

        public T Peek() {
            if (_size == 0)
                throw new InvalidOperationException("HEAP is Empty");

            return _items[0].Value;
        }

        private void RemoveAt(int index) {
            _items[index] = _items[--_size];
            _items[_size] = default(IndexedItem);
            Heapify();
            if (_size < _items.Length / 4) {
                IndexedItem[] temp = _items;
                _items = new IndexedItem[_items.Length / 2];
                Array.Copy(temp, 0, _items, 0, _size);
            }
        }

        public T Dequeue() {
            T result = Peek();
            RemoveAt(0);
            return result;
        }

        public void Enqueue(T item) {
            if (_size >= _items.Length) {
                IndexedItem[] temp = _items;
                _items = new IndexedItem[_items.Length * 2];
                Array.Copy(temp, _items, temp.Length);
            }

            int index = _size++;
            _items[index] = new IndexedItem {Value = item, Id = Interlocked.Increment(ref _count)};
            Percolate(index);
        }

        public bool Remove(T item) {
            for (var i = 0; i < _size; ++i) {
                if (EqualityComparer<T>.Default.Equals(_items[i].Value, item)) {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

    }

}