using UnityEngine;
using Hourai.Events;
using InControl;

namespace Hourai.SmashBrew {

    public class PauseGame : MonoBehaviour {

        [SerializeField]
        private InputControlTarget _pauseButton = InputControlTarget.Start;

        private Player _pausedPlayer;

        void Update() {
            if (SmashGame.Paused) {
                if (_pausedPlayer != null && !_pausedPlayer.Controller.GetControl(_pauseButton).WasPressed)
                    return;
                _pausedPlayer = null;
                SmashGame.Paused = false;
            } else {
                foreach (Player player in SmashGame.ActivePlayers) {
                    if (!player.Controller.GetControl(_pauseButton).WasPressed)
                        continue;
                    _pausedPlayer = player;
                    SmashGame.Paused = true;
                    break;
                }
            }
        }

    }

}