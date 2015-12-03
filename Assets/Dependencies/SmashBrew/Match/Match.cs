using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public static class Match {

        private static Mediator eventManager; 

        static Match() {
            eventManager = GlobalEventManager.Instance;
            _matchRules = new List<IMatchRule>();
        }

        public class MatchEvent : IEvent {

            public bool start;

        }

        public class SpawnPlayer : IEvent {

            public Player Player;
            public GameObject PlayerObject;

        }
        
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
                yield return null;
            }

            Character winner = _matchRules.Select(rule => rule.Winner).FirstOrDefault(character => character != null);

            eventManager.Publish(new MatchEvent { start = false });

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

            // Spawn players
            var currentPlayer = 0;

            var eventManager = GlobalEventManager.Instance;

            foreach(Player player in SmashGame.ActivePlayers) {
                Transform spawnPoint = Stage.GetSpawnPoint(currentPlayer);
                Character runtimeCharacter = player.Spawn(spawnPoint);
                if (runtimeCharacter == null)
                    continue;

                eventManager.Publish(new SpawnPlayer{ Player = player, PlayerObject = runtimeCharacter.gameObject });

                currentPlayer++;
            }

            eventManager.Publish(new MatchEvent { start = true });

            Game.StaticCoroutine(MatchLoop());
        }

    }

}