using System;
using System.Collections.Generic;
using HouraiTeahouse.Console;
using HouraiTeahouse.Events;
using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public class ConsoleCommands : MonoBehaviour {

        private class Command {
            private readonly string _name;
            private readonly ConsoleCommand _consoleCommand;

            public Command (string name, ConsoleCommand consoleCommand) {
                _name = name;
                _consoleCommand = consoleCommand;
            }

            public void Register() {
                GameConsole.RegisterCommand(_name, _consoleCommand);
            }

            public void Unregister() {
                GameConsole.UnregisterCommand(_name, _consoleCommand);
            }
        }

        private Command[] commands;

        void Awake() {
             commands = new[] {
                new Command("hitbox", HitboxCommand),
                new Command("language", LanguageCommand),
                new Command("kill", KillCommand),
                new Command("damage", DamageCommand)
            };
        }

        void OnEnable() {
            for (var i = 0; i < commands.Length; i++)
                commands[i].Register();
        }

        void OnDisable() {
            for (var i = 0; i < commands.Length; i++)
                commands[i].Unregister();
        }

        void ChangeHitboxes(bool state) {
            Hitbox.DrawHitboxes = state;
            GameConsole.Log("Hitbox drawing: {0}", Hitbox.DrawHitboxes);
        }

        static bool ArgLengthCheck(int count, ICollection<string> args, string name) {
            bool check = args.Count >= count;
            if (!check)
                GameConsole.Log("The command \"{0}\" requires at least {1} parameters.", name, count);
            return check;
        }

        static int? IntParse(string src) {
            int val;
            if (int.TryParse(src, out val))
                return val;
            return null;
        }

        static float? FloatParse(string src) {
            float val;
            if (float.TryParse(src, out val))
                return val;
            return null;
        }

        Player GetPlayer(string playerNumber) {
            int? playerNum = IntParse(playerNumber);
            if (playerNum != null) {
                if (playerNum <= 0 || playerNum > Player.MaxPlayers)
                    GameConsole.Log("There is no Player #{0}, try between 1 and {1}", playerNum, Player.MaxPlayers);
                else
                    return Player.GetPlayer(playerNum.Value - 1);
            }
            else {
                GameConsole.Log("The term {0} cannot be converted to a player number.", playerNumber);
            }
            return null;
        }

        void KillCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "kill"))
                return;
            Player player = GetPlayer(args[0]);
            if (player == null)
                return;
            GlobalMediator.Instance.Publish(new PlayerDieEvent {Player = player});
        }

        void DamageCommand(string[] args) {
            if (!ArgLengthCheck(2, args, "damage"))
                return;
            Player player = GetPlayer(args[0]);
            if (player == null)
                return;
            float? damage = FloatParse(args[1]);
            if (damage == null) {
                GameConsole.Log("The term {0} cannot be converted into a damage value.", args[1]);
                return;
            }
            player.PlayerObject.GetComponent<PlayerDamage>().Damage(this, damage.Value);
        }

        void TimeCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "time"))
                return;
            var timeScale = FloatParse(args[1]);
            if(timeScale == null)
                GameConsole.Log("The term {0} cannot be converted into a timescale value.", args[1]);
            else {
                TimeManager.TimeScale = timeScale.Value;
            }
        }

        void LanguageCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "language"))
                return;
            try {
                LanguageManager.Instance.LoadLanguage(args[0]);
                GameConsole.Log("Game Langauge changed to {0}", args[0]);
            }
            catch (InvalidOperationException ae) {
                GameConsole.Log(ae.Message);
            }
        }

        void HitboxCommand(string[] args) {
            if (!ArgLengthCheck(1, args, "hitbox"))
                return;
            switch (args[0].ToLower()) {
                case "enable":
                    ChangeHitboxes(true);
                    break;
                case "disable":
                    ChangeHitboxes(false);
                    break;
                case "toggle":
                    ChangeHitboxes(!Hitbox.DrawHitboxes);
                    break;
                default:
                    GameConsole.Log("Setting {0} not recognized.", args[0]);
                    break;
            }
        }
    }
}
