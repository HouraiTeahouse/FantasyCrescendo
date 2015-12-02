using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai {

	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandAttribute : Attribute {
		
		public string Command { get; private set; }

		public ConsoleCommandAttribute(string invocation) {
			if(string.IsNullOrEmpty(invocation))
				invocation = string.Empty;
			Command = invocation.ToLower();
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

		void Check() {
			if(_queue.Count <= Limit)
				return;
		    lock (this) {
		        while (_queue.Count > _limit)
		            _queue.Dequeue();
		    }
		}
	}

	public static class GameConsole {

		private static Dictionary<string, ConsoleCommand> _commands;
		
		private static FixedSizeQueue<string> _history;

		public static int HistorySize {
			get { return _history.Limit; }
			set { _history.Limit = value; }
		}

		static GameConsole() {
			_commands = new Dictionary<string, ConsoleCommand>();
		    _history = new FixedSizeQueue<string>(100);
			// Divert Debug Log messages to the GameConsole as well.
			Application.logMessageReceived += (log, stackTrace, type) => Log(log);

            AppDomain currentDomain = AppDomain.CurrentDomain;

            foreach(var assembly in currentDomain.GetAssemblies())
                AddConsoleCommands(assembly);

            // Add event to register all new loaded assemblies as well
		    AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => AddConsoleCommands(args.LoadedAssembly);
		}

		static void AddConsoleCommands(Assembly assembly) {
			if(assembly == null)
				throw new ArgumentNullException("assembly");
		    Type ccaType = typeof (ConsoleCommandAttribute);
		    IEnumerable<MethodInfo> methods = from type in assembly.GetTypes()
		                  from method in type.GetMethods()
		                  where method.IsDefined(ccaType, false)
		                  select method;
		    foreach (var method in methods) {
		        if (!method.IsStatic) {
		            Debug.Log(string.Format("Tried to create a console command from {0}, however it is not static", method.Name));
		        }
		        var cca = Attribute.GetCustomAttribute(method, ccaType) as ConsoleCommandAttribute;
		        try {
		            var cc = Delegate.CreateDelegate(typeof (ConsoleCommand), method) as ConsoleCommand;
		            RegisterCommand(cca.Command, cc);
		        } catch {
		            Debug.Log(string.Format("Tried to create a console command from {0}, however it's method signature doesn't match", method.Name));
		        }
		    }
		}

		static void UnityLog(string log, string stackTrace, LogType type) {
			Log(log);
		}

		public static void RegisterCommand(string command, ConsoleCommand callback) {
            if(callback == null)
                throw new ArgumentNullException("callback");
			if(!_commands.ContainsKey(command))
				_commands[command] = callback;
			else
				_commands[command] += callback;
		}

		public static bool UnregisterCommand(string command, ConsoleCommand callback) {
			if(!_commands.ContainsKey(command) || callback == null)
				return false;
			ConsoleCommand allCallbacks = _commands[command];
			allCallbacks -= callback;
		    if (allCallbacks == null)
		        _commands.Remove(command);
			else
				_commands[command] = allCallbacks;
			return true;
		}

		public static void Log(string message, params object[] objs) {
			if(message == null)
				message = string.Empty;
			_history.Enqueue(string.Format(message, objs));
		}

		public static void Execute(string command) {
			string[] args = command.Split(null);
			if(args.Length <= 0)
				return;
			if(!_commands.ContainsKey(args[0])) {
				Log("Command {0} does not exist.", args[0]);
				return;
			}
			ConsoleCommand cc = _commands[args[0]];
			Log(command);
			try {
				cc(args.Skip(1).ToArray());
			} catch { 
				Log("An error has occured.");
			}
		}
	}
}
