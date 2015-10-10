using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class Stock : CharacterComponent {

        private CharacterRespawn _respawn;
        private int _lives;
        public bool Alive {
            get { return Lives > 0; }
        }

        public int Lives {
            get { return _lives; }
            internal set {
                _lives = value;
                if(_respawn)
                    _respawn.enabled = Alive;
                if(Character)
                    Character.gameObject.SetActive(Alive);
            }
        }

        protected override void Start() {
            base.Start();
            Character.OnBlastZoneExit += () => Lives--;
            Character.gameObject.AddComponent<CharacterDeath>();
            _respawn = Character.gameObject.AddComponent<CharacterRespawn>();
        }

    }


    public class StockMatch : IMatchRule {

        //TODO: Remove UI Code from this class

        //TODO: cleanup

        [SerializeField]
        private int stock = 5;

        private List<Stock> characterStocks;

        public StockMatch() {
            characterStocks = new List<Stock>();
        }

        public StockMatch(int stockCount) : this() {
            stock = stockCount;
        }

        public void OnMatchUpdate() {
        }

        public bool IsFinished {
            get {
                return characterStocks.Sum(s => s.Alive ? 1 : 0) > 1;
            }
        }

        public Character Winner {
            get {
                Stock winner = null;
                foreach (var characterStock in characterStocks.Where(s => s.Alive)) {
                    if (winner == null)
                        winner = characterStock;
                    else
                        winner = characterStock.Lives > winner.Lives ? characterStock : winner;
                }
                return winner == null ? null : winner.Character;
            }
        }

        public void OnMatchStart() {
            characterStocks.Clear();
        }

        public void OnMatchEnd() {
        }

        public void OnSpawn(Character character) {
            // TODO: Remove this hack
            if (characterStocks.Count == 0)
                character.gameObject.AddComponent<TestInput>();

            var characterStock = character.gameObject.AddComponent<Stock>();
            characterStock.Lives = stock;
            characterStocks.Add(characterStock);

            character.gameObject.AddComponent<CharacterDamage>();
        }

    }

}