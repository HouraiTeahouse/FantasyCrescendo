using Hourai.Events;
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

            mediator.Subscribe<DataEvent.ChangePlayerLevelCommand>(onChangePlayerLevel);
            mediator.Subscribe<DataEvent.ChangePlayerMode>(onChangePlayerMode);
        }

        public void updateUIMode(Player.PlayerType pt) {
            GameObject levelTextParent = levelText.transform.parent.gameObject;
            var buttonText = playerModeBtn.GetComponentInChildren<Text>();
            if (buttonText == null) {
                Debug.LogError("Unable to get player slot button text.");
                return;
            }

            type = pt;
            levelTextParent.SetActive(type != Player.PlayerType.None);
            string text = type.ToString().ToUpper();
            if (type == Player.PlayerType.HumanPlayer)
                text += " " + playerNumber;
            buttonText.text = text;
        }

        public void changePlayerMode() {
            mediator.Publish(
                             new DataEvent.ChangePlayerMode { playerNum = playerNumber });
        }

        public void changeLevel(int level) {
            mediator.Publish(
                             new DataEvent.ChangePlayerLevelCommand { newLevel = level, playerNum = playerNumber });
        }

        public void onChangePlayerLevel(DataEvent.ChangePlayerLevelCommand cmd) {
            if (cmd.playerNum != playerNumber)
                return;
            levelText.text = "lv " + cmd.newLevel;
        }

        public void onChangePlayerMode(DataEvent.ChangePlayerMode cmd) {
            if (cmd.playerNum != playerNumber)
                return;
            updateUIMode(Player.GetNextType(type));
        }

        public void SetPlayerData(Player data) {
            _player = data;
        }
    }

}