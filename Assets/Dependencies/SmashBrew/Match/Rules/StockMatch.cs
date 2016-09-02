using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A Match Rule defining a Stock-based Match. AllPlayers will have a fixed number of lives to lose, via exiting
    /// the blast zone. After which they will no longer respawn, and cannot further participate. The winner is the last player
    /// standing. </summary>
    public sealed class StockMatch : MatchRule {

        Mediator _eventManager;

        /// <summary> The store of how many lives each player currently has. </summary>
        Dictionary<Player, int> _stocks;

        /// <summary> The number of stock the players start with. </summary>
        [SerializeField]
        int stock = 5;

        /// <summary> Readonly indexer for how many stocks each player has remaining. </summary>
        /// <param name="player"> the Player in question </param>
        /// <returns> the number of remaining stocks they have </returns>
        public int this[Player player] {
            get { return _stocks[player]; }
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = Mediator.Global;
            _eventManager.Subscribe<PlayerSpawnEvent>(OnSpawn);
            _eventManager.Subscribe<PlayerDieEvent>(OnPlayerDie);
            _stocks = new Dictionary<Player, int>();
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() { _eventManager.Unsubscribe<PlayerSpawnEvent>(OnSpawn); }

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            if (_stocks.Values.Count(lives => lives > 0) <= 1)
                Match.FinishMatch();
        }

        /// <summary> Gets the winner of the match. </summary>
        /// <remarks> Note this will return the player with the greatest number of remaining lives left. If there are more than
        /// player with the maximum number of stocks, no winner will be declared, and this method will return null. </remarks>
        /// <returns> the winning Player, null if it is a tie </returns>
        public override Player GetWinner() {
            if (_stocks.Count <= 0)
                return null;
            return _stocks.ArgMax();
        }

        bool RespawnCheck(Player character) {
            if (!isActiveAndEnabled || !_stocks.ContainsKey(character))
                return false;
            return _stocks[character] > 1;
        }

        /// <summary> Events callback. Called every time a Player dies. </summary>
        /// <param name="eventArgs"> the death event arguments </param>
        void OnPlayerDie(PlayerDieEvent eventArgs) {
            if (eventArgs.Revived || _stocks[eventArgs.Player] <= 0)
                return;
            _stocks[eventArgs.Player]--;
            _eventManager.Publish(new PlayerRespawnEvent {Player = eventArgs.Player});
            eventArgs.Revived = true;
        }

        /// <summary> Events callback. Called every time a Player spawns for the first time. Note this is not called when the
        /// player respawns. </summary>
        /// <param name="eventArgs"> the spawn event arguments </param>
        void OnSpawn(PlayerSpawnEvent eventArgs) {
            if (!isActiveAndEnabled)
                return;
            _stocks[eventArgs.Player] = stock;
            eventArgs.Player.PlayerObject.GetComponent<PlayerDamage>().Type = DamageType.Percent;
        }

    }

}