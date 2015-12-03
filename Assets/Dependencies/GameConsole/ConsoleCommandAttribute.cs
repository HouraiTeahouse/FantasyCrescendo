using System;

namespace Hourai.Console {

	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandAttribute : Attribute {
		
		public string Command{ get; private set; }

		public ConsoleCommandAttribute(string command) {
			if(string.IsNullOrEmpty(command))
				command = string.Empty;
			Command = command.ToLower();
		}

	}

}
