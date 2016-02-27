using HouraiTeahouse.Events;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    
    public class PlayerSlotUI : MonoBehaviour, IDataComponent<Player> {

        private Mediator mediator;
        private Player _player;

        [SerializeField]
        private Text _levelText = null;

        [SerializeField]
        private Image _playerImage = null;

        [SerializeField]
        private Button _playerModeBtn = null;

        /// <summary>
        /// Unity callback. Called once before object's first frame.
        /// </summary>
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
            var buttonText = _playerModeBtn.GetComponentInChildren<Text>();
            if (buttonText == null) {
                Debug.LogError("Unable to get player slot button text.");
                return;
            }

            buttonText.text = pt.Name.ToUpper();
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
            cmd.Player.CycleType();
            UpdateUiMode(cmd.Player.Type);
        }

        public void SetData(Player data) {
            _player = data;
        }
    }

}
