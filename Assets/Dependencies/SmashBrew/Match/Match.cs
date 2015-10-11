using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai.SmashBrew {

    public static class Match {

        static Match() {
            _matchRules = new List<IMatchRule>();
            _selected = new Player[SmashGame.MaxPlayers];
            for (var i = 0; i < SmashGame.MaxPlayers; i++)
                _selected[i] = new Player(i);
        }

        public static Player GetPlayerData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= SmashGame.MaxPlayers)
                throw new ArgumentException("playerNumber");
            return _selected[playerNumber];
        }

        public static IEnumerable<Player> Players {
            get {
                foreach (var player in _selected)
                    yield return player;
            }
        }

        public static IEnumerable<Player> ActivePlayers {
            get { return _selected.Where(player => player.Type != Player.PlayerType.Disabled); }
        }

        public static event Action OnMatchStart;
        public static event Action<Player> OnSpawnCharacter;
        
        private static List<IMatchRule> _matchRules;
        private static Player[] _selected;

        /// <summary>
        /// The current number of Players selected.
        /// </summary>
        public static int PlayerCount {
            get { return ActivePlayers.Count(); }
        }

        /// <summary>
        /// Whether or not there is currently a match being executed.
        /// </summary>
        public static bool InMatch { get; private set; }

        public static void AddMatchRule(IMatchRule rule) {
            if (rule != null)
                _matchRules.Add(rule);
        }

        private static IEnumerator MatchLoop() {
            // Declare the match to have been started
            InMatch = true;

            // Wait for the match to end
            while (!_matchRules.Any(rule => rule.IsFinished)) {
                foreach(var rule in _matchRules)
                    rule.OnMatchUpdate();
                yield return new WaitForFixedUpdate();
            }

            Character winner = _matchRules.Select(rule => rule.Winner).FirstOrDefault(character => character != null);

            foreach (var rule in _matchRules)
                rule.OnMatchEnd();

            InMatch = false;
        }

        public static void Begin() {

            // Don't restart a match if it is still in progress
            if (InMatch)
                return;

            if (Stage.Instance == null || PlayerCount > Stage.SupportedPlayerCount ) {
                throw new InvalidOperationException(
                    "Cannot start a match when there are more players participating than supported by the selected stage");
            }

            foreach (var rule in _matchRules)
                rule.OnMatchStart();

            // Spawn players
            var currentPlayer = 0;

            foreach(Player player in ActivePlayers) {
                Transform spawnPoint = Stage.GetSpawnPoint(currentPlayer);
                Character runtimeCharacter = player.Spawn(spawnPoint);
                if (runtimeCharacter == null)
                    continue;
                foreach (IMatchRule rule in _matchRules)
                    rule.OnSpawn(runtimeCharacter);

                if(OnSpawnCharacter != null)
                    OnSpawnCharacter(player);

                currentPlayer++;
            }

            if(OnMatchStart != null)
                OnMatchStart();

            Game.StaticCoroutine(MatchLoop());
        }

    }

}