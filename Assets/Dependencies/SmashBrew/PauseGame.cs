using UnityEngine;
using InControl;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// Handler MonoBehaviour that listens for button presses and pauses the game as needed.
    /// </summary>
    public class PauseGame : MonoBehaviour {
        /// <summary>
        /// The button that pauses the game.
        /// </summary>
        [SerializeField] private InputControlTarget _pauseButton = InputControlTarget.Start;

        /// <summary>
        /// The player that paused the game.
        /// </summary>
        private Player _pausedPlayer;

        /// <summary>
        /// Unity callback. Called once every frame.
        /// </summary>
        void Update() {
            if (TimeManager.Paused) {
                if (_pausedPlayer != null && !_pausedPlayer.Controller.GetControl(_pauseButton).WasPressed)
                    return;
                _pausedPlayer = null;
                TimeManager.Paused = false;
            }
            else {
                foreach (Player player in Player.ActivePlayers) {
                    if (player.Controller == null || !player.Controller.GetControl(_pauseButton).WasPressed)
                        continue;
                    _pausedPlayer = player;
                    TimeManager.Paused = true;
                    break;
                }
            }
        }
    }
}