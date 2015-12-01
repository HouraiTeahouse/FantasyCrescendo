using System;
using System.Reflection;

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

	public static class GameConsole {

		private static Dictionary<string, ConsoleCommand> _commands;

		static GameConsole() {
			_commands = new Dictionary<string, ConsoleCommand>();
		}

		static void AddConsoleCommands(Assembly assembly) {
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			foreach(
		}

		public static void Log(string message) {

		}

		public static void Execute(string command) {
		
		}
	}

}
