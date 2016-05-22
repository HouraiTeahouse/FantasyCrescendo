using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.Console {
    /// <summary>
    /// Delegate type for commands executable by GameConsole
    /// </summary>
    /// <param name="args">the Console arguments. Does not include command name</param>
    public delegate void ConsoleCommand(string[] args);

    /// <summary>
    /// A globally accessible commandline-like debug console accessible in both the Unity Editor and in builds.
    /// Allows registering bash-like commands.
    /// Useful for creating cheat modes and debug views in builds.
    /// </summary>
    public static class GameConsole {
        /// <summary>
        /// Built-in GameConsole commands
        /// </summary>
        public static class Commands {
            /// <summary>
            /// Console Comand for clearing the GameConsole history.
            /// </summary>
            /// <param name="args">Console arguments</param>
            public static void Clear(string[] args) {
                GameConsole.Clear();
            }

            /// <summary>
            /// Console command for echoing back the argument values.
            /// </summary>
            /// <param name="args">Console arguments</param>
            public static void Echo(string[] args) {
                foreach (string arg in args)
                    Log(arg);
            }
        }

        private static Dictionary<string, ConsoleCommand> _commands;
        private static FixedSizeQueue<string> _history;

        /// <summary>
        /// Called every time the Console is updated.
        /// </summary>
        public static event Action OnConsoleUpdate;

        /// <summary>
        /// How long of a history will the GameConsole maintain.
        /// </summary>
        public static int HistorySize {
            get { return _history.Limit; }
            set { _history.Limit = value; }
        }

        /// <summary>
        /// The log history of all logged messages on the GameConsole
        /// </summary>
        public static IEnumerable<string> History {
            get { return _history; }
        }

        static GameConsole() {
            Init();
        }

        private static bool init = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init() {
            if (init)
                return;
            _commands = new Dictionary<string, ConsoleCommand>();
            _history = new FixedSizeQueue<string>(100);

            // Divert Debug LogCreation messages to the GameConsole as well.
            Application.logMessageReceived += (log, stackTrace, type) => Log(log);

            RegisterCommand("echo", Commands.Echo);
            RegisterCommand("clear", Commands.Clear);
            init = true;
        }

        /// <summary>
        /// Registers a callback to a command that is executable in the GameConsole.
        /// </summary>
        /// <remarks>
        /// Multiple callbacks can be registered to the same command. All will be executed, in the order in which
        /// they were registered.
        /// </remarks>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="command"/> or <paramref name="callback"/> is null.</exception>
        /// <param name="command">the command string to use</param>
        /// <param name="callback">the handler that is to be registered</param>
        public static void RegisterCommand(string command, ConsoleCommand callback) {
            Check.ArgumentNull("callback", callback);
            Check.ArgumentNull("command", command);
            if (!_commands.ContainsKey(command))
                _commands[command] = callback;
            else
                _commands[command] += callback;
        }

        /// <summary>
        /// Unregisters a callback from a command.
        /// </summary>
        /// <remarks>
        /// If a command is registered multiple times, one call only removes one of it's instances.
        /// </remarks>
        /// <param name="command">the command string to use</param>
        /// <param name="callback">the handler that is to be registered</param>>
        /// <returns>whether the command was successfully unregistered or not</returns>
        public static bool UnregisterCommand(string command, ConsoleCommand callback) {
            if (!_commands.ContainsKey(command))
                return false;
            ConsoleCommand allCallbacks = _commands[command];
            allCallbacks -= callback;
            if (allCallbacks == null)
                _commands.Remove(command);
            else
                _commands[command] = allCallbacks;
            return true;
        }

        /// <summary>
        /// Clears Console history.
        /// </summary>
        public static void Clear() {
            _history.Clear();
            if (OnConsoleUpdate != null)
                OnConsoleUpdate();
        }

        /// <summary>
        /// Logs a message to the GameConsole
        /// </summary>
        /// <param name="message">the message, can be formatted as seen in String.Format</param>
        /// <param name="objs">the object arguments used to format the message</param>
        public static void Log(string message, params object[] objs) {
            if (message == null)
                message = string.Empty;
            _history.Enqueue(string.Format(message, objs));
            if (OnConsoleUpdate != null)
                OnConsoleUpdate();
        }

        /// <summary>
        /// Executes a console command.
        /// </summary>
        /// <param name="command">the command to execute, including arguments.</param>
        public static void Execute(string command) {
            string[] args = command.Split(null);
            if (args.Length <= 0)
                return;
            if (!_commands.ContainsKey(args[0])) {
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
