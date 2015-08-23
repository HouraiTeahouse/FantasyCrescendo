using System;
using UnityEngine;
using System.Collections;

namespace Hourai.SmashBrew {

    [EditorOnly]
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

            for(var i = 0; i < SmashGame.MaxPlayers; i++)
                Match.SetCharcter(i, testCharacters[i].data, testCharacters[i].pallete);
        }

    }
}

