using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {
    
    public sealed class CharacterMatchData {

        public readonly CharacterData Data;
        public readonly int Pallete;

        [DontSerialize, Hide]
        public int PlayerNumber { get; internal set; }

        [DontSerialize, Hide]
        public Character SpawnedInstance { get; private set; }

        internal CharacterMatchData() {
            Data = null;
            Pallete = 0;
            PlayerNumber = 0;
            SpawnedInstance = null;
        }

        internal CharacterMatchData(CharacterData data, int pallete) {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.AlternativeCount <= 0)
                throw new ArgumentException("A Character Data must have at least one pallete choice.");
            pallete = Mathf.Clamp(pallete, 0, data.AlternativeCount - 1);
            Data = data;
            Pallete = pallete;
        }

        internal Character Spawn(Transform location = null) {
            Vector3 position = location != null ? location.position : Vector3.zero;
            Quaternion rotation = location != null ? location.rotation : Quaternion.identity;

            Character prefab = Data.LoadPrefab(Pallete);
            if (prefab == null)
                return null;
            SpawnedInstance = prefab.InstantiateNew(position, rotation);
            SpawnedInstance.PlayerNumber = PlayerNumber;
            return SpawnedInstance;
        }

    }

    public static class Match {

        static Match() {
            _matchRules = new List<IMatchRule>();
            _selected = new CharacterMatchData[SmashGame.MaxPlayers];
        }

        public static CharacterMatchData GetCharacterMatchData(int playerNumber) {
            if (playerNumber < 0 || playerNumber >= SmashGame.MaxPlayers)
                throw new ArgumentException("playerNumber");
            CharacterMatchData data = _selected[playerNumber];
            if (data != null)
                data.PlayerNumber = playerNumber;
            return data;
        }

        public static event Action OnMatchStart;
        public static event Action<CharacterMatchData> OnSpawnCharacter;
        
        private static List<IMatchRule> _matchRules;
        private static CharacterMatchData[] _selected; 

        /// <summary>
        /// The current number of Players selected.
        /// </summary>
        public static int PlayerCount {
            get {
                return _selected == null ? 0 : _selected.Count(selection => selection != null);
            }
        }

        /// <summary>
        /// Whether or not there is currently a match being executed.
        /// </summary>
        [DontSerialize, Hide]
        public static bool InMatch { get; private set; }

        public static void SetCharcter(int playerNumber, CharacterData character, int pallete = 0) {
            if (character == null)
                throw new ArgumentNullException("character");
            _selected[playerNumber] = new CharacterMatchData(character, pallete);
        }

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
            int playerCount = PlayerCount;
            var currentPlayer = 0;
            for (var i = 0; i < PlayerCount; i++) {
                if (_selected[i] == null)
                    continue;
                _selected[i].PlayerNumber = i;

                Transform spawnPoint = Stage.GetSpawnPoint(currentPlayer);
                Character runtimeCharacter = _selected[i].Spawn(spawnPoint);
                if (runtimeCharacter == null)
                    continue;
                foreach (IMatchRule rule in _matchRules)
                    rule.OnSpawn(runtimeCharacter);
                OnSpawnCharacter.SafeInvoke(_selected[i]);
                currentPlayer++;
            }

            OnMatchStart.SafeInvoke();

            Game.StaticCoroutine(MatchLoop());
        }

    }

}