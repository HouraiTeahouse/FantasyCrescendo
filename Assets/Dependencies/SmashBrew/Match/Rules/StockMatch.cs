using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// A Match Rule defining a Stock-based Match.
    /// 
    /// AllPlayers will have a fixed number of lives to lose, via exiting the blast zone.
    /// After which they will no longer respawn, and cannot further participate.
    /// The winner is the last player standing.
    /// </summary>
    public sealed class StockMatch : MatchRule {

        /// <summary>
        /// The number of stock the players start with.
        /// </summary>
        [SerializeField]
        private int stock = 5;

        private Mediator _eventManager;

        /// <summary>
        /// The store of how many lives each player currently has.
        /// </summary>
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
        /// Unity Callback. Called once every frame.
        /// </summary>
        void Update() {
            if(_stocks.Values.Count(lives => lives > 0) <= 1)
                Match.FinishMatch();
        }

        /// <summary>
        /// Readonly indexer for how many stocks each player has remaining.
        /// </summary>
        /// <param name="player">the Player in question</param>
        /// <returns>the number of remaining stocks they have</returns>
        public int this[Player player] {
            get { return _stocks[player]; }
        }

        /// <summary>
        /// Gets the winner of the match.
        /// </summary>
        /// <remarks>
        /// Note this will return the player with the greatest number of remaining lives left.
        /// If there are more than player with the maximum number of stocks, no winner will be
        /// declared, and this method will return null.
        /// </remarks>
        /// <returns>the winning Player, null if it is a tie</returns>
        public override Player GetWinner() {
            Player winner = null;
            var maxStocks = int.MinValue;
            foreach (KeyValuePair<Player, int> playerStock in _stocks) {
                if (playerStock.Value < maxStocks)
                    continue;
                else if (playerStock.Value == maxStocks) {
                    // More than one player has the maximum number of lives
                    // it is a tie, don't declare a winner
                    winner = null;
                } else {
                    // a new maximum number of lives has been found
                    // declare them the winner
                    winner = playerStock.Key;
                    maxStocks = playerStock.Value;
                }
            }
            return winner;
        }

        bool RespawnCheck(Player character) {
            if (!isActiveAndEnabled || !_stocks.ContainsKey(character))
                return false;

            return _stocks[character] > 1;
        }

        /// <summary>
        /// Event callback. Called every time a Player dies.
        /// </summary>
        /// <param name="eventArgs">the death event arguments</param>
        void OnPlayerDie(PlayerDieEvent eventArgs) {
            if (eventArgs.Revived || _stocks[eventArgs.Player] <= 0)
                return;
            _stocks[eventArgs.Player]--;
            _eventManager.Publish(new PlayerRespawnEvent { Player = eventArgs.Player });
            eventArgs.Revived = true;
        }

        /// <summary>
        /// Event callback. Called every time a Player spawns for the first time.
        /// Note this is not called when the player respawns.
        /// </summary>
        /// <param name="eventArgs">the spawn event arguments</param>
        void OnSpawn(PlayerSpawnEvent eventArgs) {
            if (!isActiveAndEnabled)
                return;
            _stocks[eventArgs.Player] = stock;
            eventArgs.Player.PlayerObject.DamageType = DamageType.Percent;
        }

    }

}
