using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {
    
    public class PlayerSlotUI : MonoBehaviour, IPlayerGUIComponent {

        private Mediator mediator;
        private Player _player;

        [SerializeField]
        private Text levelText = null;

        [SerializeField]
        private Image playerImage = null;

        [SerializeField]
        private Button playerModeBtn = null;

        [SerializeField]
        private int playerNumber;
        public Player.PlayerType type = Player.PlayerType.HumanPlayer;

        private void Start() {
            DataManager dm = DataManager.Instance;
    
            if (dm == null) {
                Debug.LogError("Unable to find the data manager object.");
                Destroy(gameObject);
            }

            mediator = dm.mediator;

            if (mediator == null || levelText == null || playerModeBtn == null || playerImage == null) {
                Debug.LogError("Fill all gameObjects needed by this object.");
                Destroy(gameObject);
            }

            mediator.Subscribe<DataCommands.ChangePlayerLevelCommand>(onChangePlayerLevel);
            mediator.Subscribe<DataCommands.ChangePlayerMode>(onChangePlayerMode);
        }

        public void updateUIMode(Player.PlayerType pt) {
            GameObject levelTextParent = levelText.transform.parent.gameObject;
            var buttonText = playerModeBtn.GetComponentInChildren<Text>();
            if (buttonText == null) {
                Debug.LogError("Unable to get player slot button text.");
                return;
            }

            type = pt;
            levelTextParent.SetActive(type != Player.PlayerType.Disabled);
            switch (type) {
                case Player.PlayerType.CPU:
                    buttonText.text = "CPU";
                    break;
                case Player.PlayerType.Disabled:
                    buttonText.text = "NONE";
                    break;
                case Player.PlayerType.HumanPlayer:
                    buttonText.text = "PLAYER " + (playerNumber + 1);
                    break;
                default:
                    Debug.LogError("Invalid player type in player slot.");
                    break;
            }
        }

        public void changePlayerMode() {
            mediator.Publish(
                             new DataCommands.ChangePlayerMode { playerNum = playerNumber });
        }

        public void changeLevel(int level) {
            mediator.Publish(
                             new DataCommands.ChangePlayerLevelCommand { newLevel = level, playerNum = playerNumber });
        }

        public void onChangePlayerLevel(DataCommands.ChangePlayerLevelCommand cmd) {
            if (cmd.playerNum != playerNumber)
                return;
            levelText.text = "lv " + cmd.newLevel;
        }

        public void onChangePlayerMode(DataCommands.ChangePlayerMode cmd) {
            if (cmd.playerNum != playerNumber)
                return;
            updateUIMode(Player.GetNextType(type));
        }

        public void SetPlayerData(Player data) {
            _player = data;
        }
    }

}