using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class PauseGame : MonoBehaviour {

        [SerializeField]
        private string _pauseButton = "Pause";
        private Player _pausedPlayer;

        void Update() {
            if (SmashGame.Paused) {
                if (_pausedPlayer != null && !_pausedPlayer.Controller.GetButton(_pauseButton).GetButtonValue())
                    return;
                _pausedPlayer = null;
                SmashGame.Paused = false;
            } else {
                foreach (Player player in SmashGame.ActivePlayers) {
                    if (!player.Controller.GetButton(_pauseButton).GetButtonValue())
                        continue;
                    _pausedPlayer = player;
                    SmashGame.Paused = true;
                    break;
                }
            }
        }

    }

}