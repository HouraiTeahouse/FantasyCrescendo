using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    /// <summary>
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public sealed class Match : Singleton<Match> {

        private struct Selection {

            public CharacterData CharacterData;
            public int Pallete;

            public Character Load() {
                return CharacterData.LoadPrefab(Pallete);
            }

        }

        [Serialize]
        private static List<IMatchRule> _matchRules;

        [Serialize]
        private List<Selection> selectedCharacters;

        public static int PlayerCount {
            get { return Selected == null ? 0 : Selected.Count; }
        }

        private static List<Selection> Selected {
            get { return Instance == null ? null : Instance.selectedCharacters; }
        }

        public static void SetCharcter(int playerNumber, CharacterData character, int pallete = 1) {
            if (character == null)
                throw new ArgumentNullException("character");
            if(Instance == null)
                throw new InvalidOperationException();
            Selected[playerNumber] = new Selection {
                CharacterData = character, Pallete = pallete
            };
        }

        private static IEnumerator MatchLoop() {
            // Wait for the match to end
            while (!_matchRules.Any(rule => rule.IsFinished)) {
                foreach(var rule in _matchRules)
                    rule.OnMatchUpdate();
                yield return new WaitForEndOfFrame();
            }

            Character winner = _matchRules.Select(rule => rule.Winner).FirstOrDefault(character => character != null);

            foreach (var rule in _matchRules)
                rule.OnMatchEnd();
        }

        public static void Begin() {
            if (Instance == null || Stage.Instance == null || PlayerCount > Stage.SupportedPlayerCount) {
                throw new InvalidOperationException(
                    "Cannot start a match when there are more players participating than supported by the selected stage");
            }

            foreach (var rule in _matchRules)
                rule.OnMatchStart();

            // Spawn players
            for (var i = 0; i < PlayerCount; i++) {
                Character runtimeCharacter = SmashGame.SpawnPlayer(i, Selected[i].Load());
                foreach(var rule in _matchRules)
                    rule.OnSpawn(runtimeCharacter);
                Transform spawnPoint = Stage.GetSpawnPoint(i);
                runtimeCharacter.position = spawnPoint.position;
                runtimeCharacter.rotation = spawnPoint.rotation;
            }

            Instance.StartCoroutine(MatchLoop());
        }

    }

}