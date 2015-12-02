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

		public void GetEnumerator() {
			return _queue.GetEnumerator();
		}

		IEnumerable.GetEnumerator() {
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
			return _queue.Pop();
		}

		public void Clear() {
			_queue.Clear();
		}

		void Check() {
			if(count <= Limit)
				return;
			lock(this) {
				T overflow;
				while(_queue.Count > _limit && _queue.TryDequeue(out overflow));
			}
		}
	}
}
