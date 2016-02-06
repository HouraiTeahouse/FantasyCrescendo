using System;
using UnityEngine;

namespace Hourai.SmashBrew {
    
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
            foreach (var player in SmashGame.Players)
            {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.Character = testCharacters[index].Data;
                player.Pallete = testCharacters[index].Pallete;
                player.Type = Player.PlayerType.HumanPlayer;
                index++;
            }
        }
    }
}

