using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class CharacterLoader : MonoBehaviour {

        private DataManager dataManager;

        [SerializeField]
        private GameObject characterSlotPrefab;

        [SerializeField]
        private GameObject characterPanel;

        [SerializeField]
        private GameObject playerSlotPrefab;

        [SerializeField]
        private GameObject playerSlotPanel;

        // Use this for initialization
        private void Start() {
            if (characterSlotPrefab == null || characterPanel == null || playerSlotPanel == null || playerSlotPrefab == null) {
                Debug.LogError("Please fill all gameobjects needed by this component.");
                return;
            }

            dataManager = DataManager.Instance;
            if (dataManager == null) {
                Debug.LogError("Couldn't find the data manager component in the data manager object.");
                return;
            }

            fillPanel();
            fillPlayerSlots();
        }

        public void selectCharacter(int playerNum, string characterName) {
            Debug.Log(playerNum + "  " + characterName);
        }

        public void fillPanel() {
            foreach (var character in dataManager.getAvailableCharacters()) {
                GameObject go = Instantiate(characterSlotPrefab);
                var text = go.GetComponentInChildren<Text>();
                if (text == null) {
                    Debug.LogError("Couldn't find the text component in the character slot.");
                    return;
                }
                text.text = character.FirstName;
                go.transform.SetParent(characterPanel.transform, false);
            }
        }

        public void fillPlayerSlots() {
            foreach(Player player in SmashGame.Players) {
                GameObject go = Instantiate(playerSlotPrefab);
                var psu = go.GetComponent<PlayerSlotUI>();
                if (psu == null) {
                    Debug.LogError("The player slot object should have a PlayerSlotUI component.");
                    return;
                }
                psu.updateUIMode(player.Type);
                psu.SetPlayerData(player);

                go.transform.SetParent(playerSlotPanel.transform, false);
            }
        }

    }
}
