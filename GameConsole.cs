using System;
using System.Reflection;
using System.Collecitons.Generic;
using UnityEngine;

namespace Hourai {

	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandAttribute : Attribute {
		
		public string InvocationKey { get; private set; }

		public ConsoleCommandAttribute(string invocation) {
			if(string.IsNullOrEmpty(invocation))
				invocation = string.empty;
			InvocationKey = invocation.ToLower();
		}

	}

	public delegate void ConsoleCommand(string[] args);

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

		void Check() {
			if(count <= Limit)
				return;
			lock(this) {
				T overflow;
				while(_queue.Count > _limit && _queue.TryDequeue(out overflow));
			}
		}
	}

	public static class GameConsole {

		private static Dictionary<string, ConsoleCommand> _commands;
		
		private static FixedQueue<string> _history;

		public static int HistorySize {
			get { return _history.Limit; }
			set { history.Limit = value; }
		}

		static GameConsole() {
			_commands = new Dictionary<string, ConsoleCommand>();
			_history = new FixedQueue<string>(100
			// Divert Debug Log messages to the GameConsole as well.
			Application.logMessageRecieved += (log, stackTrace, type) => Log(log);
		}

		static void AddConsoleCommands(Assembly assembly) {
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			foreach(
		}

		void UnityLog(string log, string stackTrace, LogType type) {
			Log(message);
		}

		public static void Log(string message) {

		}

		public static void Execute(string command) {
			
		}
	}

}
