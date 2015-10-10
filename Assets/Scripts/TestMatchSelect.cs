using System;
using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {
    
    public class TestMatchSelect : MonoBehaviour {

        [Serializable]
        private class Selection {

            public CharacterData data;
            public int pallete;

        }

        [SerializeField]
        private Selection[] testCharacters;

        [SerializeField]
        private int stockCount = 5;
        
        void Awake() {
            Match.AddMatchRule(new StockMatch(stockCount));

            var index = 0;
            foreach (var player in Match.Players)
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

