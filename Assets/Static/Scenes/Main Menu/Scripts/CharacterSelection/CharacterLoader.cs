using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class CharacterLoader : MonoBehaviour {

        private DataManager _dataManager;

        [SerializeField]
        private GameObject _characterSlotPrefab;

        [SerializeField]
        private GameObject _characterPanel;

        [SerializeField]
        private GameObject _playerSlotPrefab;

        [SerializeField]
        private GameObject _playerSlotPanel;

        // Use this for initialization
        private void Start() {
            if (_characterSlotPrefab == null || _characterPanel == null || _playerSlotPanel == null || _playerSlotPrefab == null) {
                Debug.LogError("Please fill all gameobjects needed by this component.");
                return;
            }

            _dataManager = DataManager.Instance;
            if (_dataManager == null) {
                Debug.LogError("Couldn't find the data manager component in the data manager object.");
                return;
            }

            FillPanel();
            FillPlayerSlots();
        }

        public void SelectCharacter(int playerNum, string characterName) {
            Debug.Log(playerNum + "  " + characterName);
        }

        public void FillPanel() {
            foreach (var character in _dataManager.GetAvailableCharacters()) {
                GameObject go = Instantiate(_characterSlotPrefab);
                var text = go.GetComponentInChildren<Text>();
                if (text == null) {
                    Debug.LogError("Couldn't find the text component in the character slot.");
                    return;
                }
                text.text = character.FirstName;
                go.transform.SetParent(_characterPanel.transform, false);
            }
        }

        public void FillPlayerSlots() {
            foreach(Player player in SmashGame.Players) {
                GameObject go = Instantiate(_playerSlotPrefab);
                var psu = go.GetComponent<PlayerSlotUI>();
                if (psu == null) {
                    Debug.LogError("The player slot object should have a PlayerSlotUI component.");
                    return;
                }
                psu.UpdateUiMode(player.Type);
                psu.SetPlayerData(player);

                go.transform.SetParent(_playerSlotPanel.transform, false);
            }
        }

    }
}
