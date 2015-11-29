using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Hourai.SmashBrew {

    public static class Match {

        static Match() {
            _matchRules = new List<IMatchRule>();
        }

        public static event Action OnMatchStart;
        public static event Action<Player> OnSpawnCharacter;
        
        private static readonly List<IMatchRule> _matchRules;

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

            if (Stage.Instance == null || SmashGame.ActivePlayerCount > Stage.SupportedPlayerCount ) {
                throw new InvalidOperationException(
                    "Cannot start a match when there are more players participating than supported by the selected stage");
            }

            foreach (var rule in _matchRules)
                rule.OnMatchStart();

            // Spawn players
            var currentPlayer = 0;

            foreach(Player player in SmashGame.ActivePlayers) {
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