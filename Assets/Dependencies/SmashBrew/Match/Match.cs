using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {
    

    public sealed class Match : Singleton<Match> {

        public static Tuple<CharacterData, int> GetCharacterMatchData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= SmashGame.MaxPlayers)
                throw new ArgumentException("playerNumber");
            return _selected[playerNumber];
        }

        public static event Action OnMatchStart;

        [Serialize]
        private static List<IMatchRule> _matchRules;

        [Serialize]
        private static Tuple<CharacterData, int>[] _selected; 

        /// <summary>
        /// The current number of Players selected.
        /// </summary>
        public static int PlayerCount {
            get {
                if (_selected == null)
                    return 0;
                return _selected.Count(selection => selection.Item1 != null);
            }
        }

        /// <summary>
        /// Whether or not there is currently a match being executed.
        /// </summary>
        [DontSerialize, Hide]
        public static bool InMatch { get; private set; }

        /// <summary>
        /// Called on instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            int playerCount = SmashGame.MaxPlayers;
            if (_selected == null) {
                _selected = new Tuple<CharacterData, int>[playerCount];
            } else if (_selected.Length != playerCount) {
                var temp = new Tuple<CharacterData, int>[playerCount];
                Array.Copy(_selected, temp, Mathf.Min(playerCount, _selected.Length));
                _selected = temp;
            }
        }

        public static void SetCharcter(int playerNumber, CharacterData character, int pallete = 1) {
            if (character == null)
                throw new ArgumentNullException("character");
            if(Instance == null)
                throw new InvalidOperationException();
            _selected[playerNumber] = new Tuple<CharacterData, int>(character, pallete);
        }

        /// <summary>
        /// Called on instance destruction.
        /// </summary>
        private void OnDestroy() {
            InMatch = false;
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

            if (Instance == null || Stage.Instance == null || PlayerCount > Stage.SupportedPlayerCount ) {
                throw new InvalidOperationException(
                    "Cannot start a match when there are more players participating than supported by the selected stage");
            }

            foreach (var rule in _matchRules)
                rule.OnMatchStart();

            // Spawn players
            int playerCount = PlayerCount;
            var currentPlayer = 0;
            for (var i = 0; i < PlayerCount; i++) {
                CharacterData data = _selected[i].Item1;
                if (data == null)
                    continue;
                Character prefab = data.LoadPrefab(_selected[i].Item2);
                Character runtimeCharacter = SmashGame.SpawnPlayer(i, prefab);
                Transform spawnPoint = Stage.GetSpawnPoint(currentPlayer);
                runtimeCharacter.position = spawnPoint.position;
                runtimeCharacter.rotation = spawnPoint.rotation;
                foreach (IMatchRule rule in _matchRules)
                    rule.OnSpawn(runtimeCharacter);
                currentPlayer++;
            }

            OnMatchStart.SafeInvoke();

            Instance.StartCoroutine(MatchLoop());
        }

    }

}