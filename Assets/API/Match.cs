using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Genso.API {


    /// <summary>
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public abstract class Match : GensoBehaviour
    {

        private List<CharacterData> selectedCharacters;
		private static bool _isFinished;
		private static int _winner;
		
		public int PlayerCount
		{
			get { return selectedCharacters.Count; }
		}

		protected virtual void Awake()
        {
            selectedCharacters = new List<CharacterData>();
        }

		public void SetCharcter(int playerNumber, CharacterData character)
        {
            if (character == null)
                throw new ArgumentNullException("character");
            selectedCharacters[playerNumber] = character;
        }

		public void SetCharacters(IEnumerable<CharacterData> characters)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");
            selectedCharacters.Clear();
            selectedCharacters.AddRange(characters);
        }

		/// <summary>
		/// Finishes the match and declares a winner
		/// </summary>
		/// <param name="playerNumber">Player number.</param>
		public static void DeclareWinner(int playerNumber) {
			_isFinished  = true;
			_winner = playerNumber;
		}

		IEnumerator MatchLoop() {
			// Wait for the match to end
			while(!_isFinished) {
				yield return new WaitForEndOfFrame();
			}

			// Check for tie
			if(_winner < 0 || _winner >= PlayerCount) {
				// Initiate Sudden Death
			} else {
				// Declare winner
			}
		}

		void OnDestroy() {
			// On destruction of a match object, reset the winner
			_isFinished = false;
			_winner = -1;
		}

        void OnLevelWasLoaded(int level)
        {
            Debug.Log("Hello");

			if(Stage.Instance != null)
				return;

		    StartMatch();

		}

		public void StartMatch() {
			if (PlayerCount > Stage.SupportedPlayerCount)
				throw new InvalidOperationException("Cannot start a match when there are more players participating than supported by the selected stage");
            
            OnStartMatch();

			// Spawn players
			for(int i = 0; i < PlayerCount; i++) {
				Character runtimeCharacter = Game.SpawnPlayer(i,selectedCharacters[i]);
				OnSpawn(runtimeCharacter);
				Stage.GetSpawnPoint(i).EditSpawn(runtimeCharacter);
			}

			StartCoroutine(MatchLoop());
		}

        protected virtual void OnStartMatch() {
        }

		protected abstract void OnSpawn(Character character);

    }


}