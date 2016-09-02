using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> Handler MonoBehaviour that listens for button presses and pauses the game as needed. </summary>
    public class PauseGame : MonoBehaviour {

        /// <summary> The button that pauses the game. </summary>
        [SerializeField]
        InputTarget _pauseButton = InputTarget.Start;

        /// <summary> Unity callback. Called once every frame. </summary>
        void Update() {
            if (TimeManager.Paused) {
                Assert.IsNotNull(SmashTimeManager.PausedPlayer);
                Player player = SmashTimeManager.PausedPlayer;
                if (player != null && !player.Controller.GetControl(_pauseButton).WasPressed)
                    return;
                SmashTimeManager.PausedPlayer = null;
            }
            else {
                foreach (Player player in Player.ActivePlayers) {
                    if (player.Controller == null || !player.Controller.GetControl(_pauseButton).WasPressed)
                        continue;
                    SmashTimeManager.PausedPlayer = player;
                    break;
                }
            }
        }

    }

}