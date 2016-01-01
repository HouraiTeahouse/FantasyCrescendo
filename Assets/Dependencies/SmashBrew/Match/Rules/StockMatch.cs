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

        private Mediator _eventManager;
        private Dictionary<Player, int> _stocks;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<PlayerSpawnEvent>(OnSpawn);
            _eventManager.Subscribe<PlayerDieEvent>(OnPlayerDie);
            _stocks = new Dictionary<Player, int>();
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
           _eventManager.Unsubscribe<PlayerSpawnEvent>(OnSpawn);
        }

        /// <summary>
        /// Is the Match finished?
        /// </summary>
        protected override bool IsFinished {
            get {
                return _stocks.Values.Sum(s => (s > 0) ? 1 : 0) > 1;
            }
        }

        /// <summary>
        /// Readonly indexer for how many stocks each player has remaining.
        /// </summary>
        /// <param name="player">the Player in question</param>
        /// <returns>the number of remaining stocks they have</returns>
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

        void OnPlayerDie(PlayerDieEvent eventArgs) {
            if (eventArgs.Revived || _stocks[eventArgs.Player] <= 0)
                return;
            _stocks[eventArgs.Player]--;
            _eventManager.Publish(new RespawnEvent { Player = eventArgs.Player });
            eventArgs.Revived = true;
        }

        void OnSpawn(PlayerSpawnEvent eventArgs) {
            if (!isActiveAndEnabled)
                return;
            _stocks[eventArgs.Player] = stock;
        }

    }

}