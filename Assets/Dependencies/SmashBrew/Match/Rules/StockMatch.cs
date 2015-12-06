using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class StockMatch : MatchRule {

        //TODO: Remove UI Code from this class

        //TODO: cleanup

        [SerializeField]
        private int stock = 5;

        private Mediator eventManager;
        private Respawn _respawn;
        private Dictionary<Player, int> _stocks;

        protected override void Awake() {
            base.Awake();
            eventManager = GlobalEventManager.Instance;
            eventManager.Subscribe<SpawnPlayerEvent>(OnSpawn);
            eventManager.Subscribe<RespawnEvent>(OnRespawn);
            _respawn = FindObjectOfType<Respawn>();
            _respawn.ShouldRespwan += RespawnCheck;
            _stocks = new Dictionary<Player, int>();
        }

        void OnDestroy() {
           eventManager.Unsubscribe<SpawnPlayerEvent>(OnSpawn);
        }

        protected override bool IsFinished {
            get {
                return _stocks.Values.Sum(s => (s > 0) ? 1 : 0) > 1;
            }
        }

        public int this[Player player] {
            get { return _stocks[player]; }
        }

        public override Player Winner {
            get {
                Player winner = null;
                foreach (KeyValuePair<Player, int> characterStock in _stocks.Where(characterStock => characterStock.Value > 0)) {
                    if (winner == null)
                        winner = characterStock.Key;
                    else
                        winner = (characterStock.Value > _stocks[winner]) ? characterStock.Key : winner;
                }
                return winner;
            }
        }

        bool RespawnCheck(Player character) {
            if (!isActiveAndEnabled || !_stocks.ContainsKey(character))
                return false;

            return _stocks[character] > 1;
        }

        void OnRespawn(RespawnEvent eventArgs) {
            if (!isActiveAndEnabled || !_stocks.ContainsKey(eventArgs.Player))
                return;
            _stocks[eventArgs.Player]--;
        }

        void OnSpawn(SpawnPlayerEvent eventArgs) {
            if (!isActiveAndEnabled)
                return;
            _stocks[eventArgs.Player] = stock;
        }

    }

}