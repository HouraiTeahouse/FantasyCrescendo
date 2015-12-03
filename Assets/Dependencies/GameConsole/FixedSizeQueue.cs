using System.Collections;
using System.Collections.Generic;

namespace Hourai.Console {

	public class FixedSizeQueue<T> : IEnumerable<T> {

		private readonly Queue<T> _queue;
		private int _limit;

		public int Limit {
		       	get { return _limit; }
		       	set {
				_limit = value;
				Check();
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return _queue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public FixedSizeQueue(int size, IEnumerable<T> collection = null) {
			_limit = size;
			_queue = new Queue<T>();
			var count = 0;
			if(collection == null)
				return;
			foreach(var obj in collection)
			       Enqueue(obj);	
		}

		public void Enqueue(T obj) {
			_queue.Enqueue(obj);
			Check();
		}

		public T Dequeue() {
			return _queue.Dequeue();
		}

		public void Clear() {
			_queue.Clear();
		}

		void Check() {
			if(_queue.Count <= Limit)
				return;
			lock(this) {
				while(_queue.Count > _limit) _queue.Dequeue();
			}
		}
	}
}
