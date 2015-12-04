using System;
using UnityEngine;

namespace Hourai.SmashBrew {
    
    public class TestMatchSelect : MonoBehaviour {

        [Serializable]
        private class Selection {

            public CharacterData data;
            public int pallete;

        }

        [SerializeField]
        private Selection[] testCharacters;

        void Awake() {
            var index = 0;
            foreach (var player in SmashGame.Players)
            {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.Character = testCharacters[index].data;
                player.Pallete = testCharacters[index].pallete;
                player.Type = Player.PlayerType.HumanPlayer;
                index++;
            }
        }
    }
}

