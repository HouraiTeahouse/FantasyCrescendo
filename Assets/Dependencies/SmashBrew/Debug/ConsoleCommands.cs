using HouraiTeahouse.Console;
using UnityEngine;
using InControl;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// Debug mode to turn on and off the rendering of hitboxes
    /// </summary>
    public class ConsoleCommands : MonoBehaviour {

        void OnEnable() {
            GameConsole.RegisterCommand("hitbox", HitboxCommand);
        }

        void OnDisable() {
            GameConsole.UnregisterCommand("hitbox", HitboxCommand);
        }

        void ChangeHitboxes(bool state) {
            Hitbox.DrawHitboxes = state;
            GameConsole.Log("Hitbox drawing: {0}", Hitbox.DrawHitboxes);
        }

        void HitboxCommand(string[] args) {
            if (args.Length < 1) {
                GameConsole.Log("Hitbox requires additional parameters. Try \"hitbox enable\", \"hitbox disable\" or \"hitbox toggle\"");
                return;
            }
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
                    GameConsole.Log("Setting {0} not recognized.Try \"hitbox enable\", \"hitbox disable\" or \"hitbox toggle\"", args[0]);
                    break;
            }
        }

    }
}
