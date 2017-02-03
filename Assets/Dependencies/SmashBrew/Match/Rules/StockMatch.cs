using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> A Match Rule defining a Stock-based Match. AllPlayers will have a fixed number of lives to lose, via exiting
    /// the blast zone. After which they will no longer respawn, and cannot further participate. The winner is the last player
    /// standing. </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Stock Match")]
    public sealed class StockMatch : MatchRule {

        Mediator _eventManager;

        /// <summary> The store of how many lijes each player currently has. </summary>
        [SerializeField]
        SyncListInt _stocks = new SyncListInt();

        /// <summary> The number of stock the players start with. </summary>
        [SerializeField]
        int stock = 5;

        /// <summary> Readonly indexer for how many stocks each player has remaining. </summary>
        /// <param name="player"> the Player in question </param>
        /// <returns> the number of remaining stocks they have </returns>
        public int this[Player player] {
            get { return _stocks[player.ID]; }
        }

        public event Action StockChanged;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = Mediator.Global;
            _stocks.Callback+= (op, index) => StockChanged.SafeInvoke();
        }

        public override void OnStartServer() {
            _stocks.Clear();
            IsActive = true;
            foreach (var player in PlayerManager.Instance.MatchPlayers) {
                _stocks.Add(-1);
                player.Changed += () => {
                    if (player.Type.IsActive && _stocks[player.ID] < 0)
                        _stocks[player.ID] = stock;
                };
            }
        }

        protected override void Start() {
            base.Start();
            _eventManager.Subscribe<PlayerSpawnEvent>(OnSpawn);
            _eventManager.Subscribe<PlayerDieEvent>(OnPlayerDie);
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() {
            _eventManager.Unsubscribe<PlayerSpawnEvent>(OnSpawn);
            _eventManager.Unsubscribe<PlayerDieEvent>(OnPlayerDie);
        }

        /// <summary> Unity Callback. Called once every frame. </summary>
        void Update() {
            if (!IsActive)
                return;
            if (hasAuthority && 
                PlayerManager.Instance.MatchPlayers.Count(p => p.Type.IsActive) > 1 &&
                _stocks.Count(lives => lives > 0) <= 1)
                Match.CmdFinishMatch(false);
        }

        /// <summary> Gets the winner of the match. </summary>
        /// <remarks> Note this will return the player with the greatest number of remaining lives left. If there are more than
        /// player with the maximum number of stocks, no winner will be declared, and this method will return null. </remarks>
        /// <returns> the winning Player, null if it is a tie </returns>
        public override Player GetWinner() {
            if (_stocks.Count <= 0)
                return null;
            return PlayerManager.Instance.MatchPlayers.Get(_stocks.ArgMax());
        }

        bool RespawnCheck(Player character) {
            if (!IsActive || !Check.Range(character.ID, _stocks))
                return false;
            return _stocks[character.ID] > 1;
        }

        /// <summary> Events callback. Called every time a Player dies. </summary>
        /// <param name="eventArgs"> the death event arguments </param>
        void OnPlayerDie(PlayerDieEvent eventArgs) {
            _stocks[eventArgs.Player.ID]--;
            if (eventArgs.Revived || _stocks[eventArgs.Player.ID] <= 0)
                return;
            _eventManager.Publish(new PlayerRespawnEvent {Player = eventArgs.Player});
            eventArgs.Revived = true;
        }

        /// <summary> Events callback. Called every time a Player spawns for the first time. Note this is not called when the
        /// player respawns. </summary>
        /// <param name="eventArgs"> the spawn event arguments </param>
        void OnSpawn(PlayerSpawnEvent eventArgs) {
            if (!isActiveAndEnabled)
                return;
            _stocks[eventArgs.Player.ID] = stock;
            //eventArgs.Player.PlayerObject.GetComponent<DamageState>().Type = DamageType.Percent;
        }

    }

}
