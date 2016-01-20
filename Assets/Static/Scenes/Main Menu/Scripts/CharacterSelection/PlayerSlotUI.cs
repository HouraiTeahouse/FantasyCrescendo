using Hourai.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {
    
    public class PlayerSlotUI : MonoBehaviour, IPlayerGUIComponent {

        private Mediator mediator;
        private Player _player;

        [SerializeField]
        private Text _levelText = null;

        [SerializeField]
        private Image _playerImage = null;

        [SerializeField]
        private Button _playerModeBtn = null;

        public Player.PlayerType Type = Player.PlayerType.HumanPlayer;

        void Start() {
            DataManager dm = DataManager.Instance;
    
            if (dm == null) {
                Debug.LogError("Unable to find the data manager object.");
                Destroy(gameObject);
                return;
            }

            mediator = dm.Mediator;

            if (mediator == null || _levelText == null || _playerModeBtn == null || _playerImage == null) {
                Debug.LogError("Fill all gameObjects needed by this object.");
                Destroy(gameObject);
            }

            if (mediator == null)
                return;
            mediator.Subscribe<DataEvent.ChangePlayerLevelCommand>(OnChangePlayerLevel);
            mediator.Subscribe<DataEvent.ChangePlayerMode>(OnChangePlayerMode);
        }

        public void UpdateUiMode(Player.PlayerType pt) {
            GameObject levelTextParent = _levelText.transform.parent.gameObject;
            var buttonText = _playerModeBtn.GetComponentInChildren<Text>();
            if (buttonText == null) {
                Debug.LogError("Unable to get player slot button text.");
                return;
            }

            Type = pt;
            levelTextParent.SetActive(Type != Player.PlayerType.None);
            string text = Type.ToString().ToUpper();
            if (Type == Player.PlayerType.HumanPlayer)
                text += " " + _player.PlayerNumber;
            buttonText.text = text;
        }

        public void ChangePlayerMode() {
            mediator.Publish(
                             new DataEvent.ChangePlayerMode { Player = _player });
        }

        public void ChangeLevel(int level) {
            mediator.Publish(
                             new DataEvent.ChangePlayerLevelCommand { Player = _player });
        }

        public void OnChangePlayerLevel(DataEvent.ChangePlayerLevelCommand cmd) {
            if (cmd.Player != _player)
                return;
            _levelText.text = "lv " + cmd.Player.CPULevel;
        }

        public void OnChangePlayerMode(DataEvent.ChangePlayerMode cmd) {
            if (cmd.Player != _player)
                return;
            UpdateUiMode(Player.GetNextType(Type));
        }

        public void SetPlayerData(Player data) {
            _player = data;
        }
    }

}