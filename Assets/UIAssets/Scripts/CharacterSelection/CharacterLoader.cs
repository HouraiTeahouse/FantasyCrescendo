using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class CharacterLoader : MonoBehaviour {

        public GameObject characterSlotPanel = null;
        public GameObject chrPanelToFill = null;
        private DataManager dataManager;
        public GameObject playerSlotPanel = null;
        public GameObject plyPanelToFill = null;

        // Use this for initialization
        private void Start() {
            if (characterSlotPanel == null || chrPanelToFill == null || plyPanelToFill == null || playerSlotPanel == null) {
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
            var i = 0;
            foreach (var character in dataManager.getAvailableCharacters()) {
                GameObject go = Instantiate(characterSlotPanel);
                var text = go.GetComponentInChildren<Text>();
                if (text == null) {
                    Debug.LogError("Couldn't find the text component in the character slot.");
                    return;
                }
                text.text = character.FirstName;
                go.transform.SetParent(chrPanelToFill.transform, false);
            }
        }

        public void fillPlayerSlots() {
            var i = 0;
            foreach(Player player in Match.Players) {
                GameObject go = Instantiate(playerSlotPanel);
                var psu = go.GetComponent<PlayerSlotUI>();
                if (psu == null) {
                    Debug.LogError("The player slot object should have a PlayerSlotUI component.");
                    return;
                }
                psu.updateUIMode(player.Type);
                psu.SetPlayerData(player);

                go.transform.SetParent(plyPanelToFill.transform, false);
            }
        }

    }
}
