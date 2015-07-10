using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.Collections.Generic;

namespace Genso.API {


    /// <summary>
    /// </summary>
    /// Author: James Liu
    /// Authored on: 07/01/2015
    public abstract class Match : GensoBehaviour
    {

        private List<Character> selectedCharacters;

        protected virtual void Awake()
        {
            selectedCharacters = new List<Character>();
        }

        public int PlayerCount
        {
            get { return selectedCharacters.Count; }
        }

        public void SetCharcter(int playerNumber, Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");
            selectedCharacters[playerNumber] = character;
        }

        public void SetCharacters(IEnumerable<Character> characters)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");
            selectedCharacters.Clear();
            selectedCharacters.AddRange(characters);
        }

        public Character SpawnCharacter(int playerNumber, Vector3 position)
        {
            Character runtimeCharacter = InstantiateCharacter(playerNumber);
            runtimeCharacter.transform.position = position;
            return runtimeCharacter;
        }

        protected Character GetCharacterPrefab(int playerNumber)
        {
            return selectedCharacters[playerNumber];
        }

        protected virtual Character InstantiateCharacter(int playerNumber)
        {
            return Instantiate(GetCharacterPrefab(playerNumber));
        }

    }


}