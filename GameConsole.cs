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

	public static class GameConsole {

		public static class Commands {

			[ConsoleCommand("clear")]
			public static void Clear(string[] args) {
				GameConsole.Clear();
			}

			[ConsoleCommand("echo")]
			public static void Echo(string[] args) {
				foreach(string arg in args)
					GameConsole.Log(arg);
			}

		}

		private static Dictionary<string, ConsoleCommand> _commands;
		private static FixedQueue<string> _history;

		public static event Action OnConsoleUpdate;

		public static int HistorySize {
			get { return _history.Limit; }
			set { history.Limit = value; }
		}

		public static IEnumerable History {
			get { return _history; }
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

		public static void RegisterCommand(string command, CommandCommand callback) {
			if(!_commands.HasKey(command))
				_commands[command] = callback;
			else
				_commands[command] += callback;
		}

		public static bool UnregisterCommand(string command, ConsoleCommand callback) {
			if(!_commands.HasKey(command))
				return false;
			ConsoleCommand allCallbacks = _commands[command];
			allCallbacks -= callback;
			if(allCallbacks == null)
				_commands.Remove(command)
			else
				_commands[command] = allCallbacks;
			return true;
		}

		public void Clear() {
			_history.Clear();
			if(OnConsoleUpdate != null)
				OnConsoleUpdate();
		}

		public static void Log(string message, params object[] objs) {
			if(message == null)
				message = string.Empty;
			_history.Enqueue(string.Format(message, objs));
			if(OnConsoleUpdate != null)
				OnConsoleUpdate();
		}

		public static void Execute(string command) {
			string[] args = command.Split(null);
			if(args.Length <= 0)
				return;
			if(!_commands.HasKey(args[0])) {
				Log("Command {0} does not exist.", args[0]);
				return;
			}
			ConsoleCommand command = _commands[args[0]];
			Log(command);
			try {
				command(args.Skip(1).ToArray());
			} catch { 
				Log("An error has occured.");
			}
		}
	}
}
