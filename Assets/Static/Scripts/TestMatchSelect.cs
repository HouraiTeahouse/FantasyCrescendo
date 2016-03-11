using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    
    public class TestMatchSelect : MonoBehaviour {

        [Serializable]
        private class Selection {

#pragma warning disable 0649
            public CharacterData Data;
            public int Pallete;
#pragma warning restore 0649

        }

        [SerializeField]
        private Selection[] testCharacters;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            var index = 0;
            foreach (var player in Player.AllPlayers) {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.SelectedCharacter = testCharacters[index].Data;
                player.Pallete = testCharacters[index].Pallete;
                player.Type = player.SelectedCharacter ? Player.PlayerType.HumanPlayer : Player.PlayerType.None;
                index++;
            }
        }
    }
}

