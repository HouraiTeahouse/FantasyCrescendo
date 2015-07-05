using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.Collections.Generic;

/// <summary>
/// </summary>
/// Author: James Liu
/// Authored on: 07/01/2015
public abstract class Match : Singleton<Match> {

    private List<Character> selectedCharacters;

    protected override void Awake() {
        base.Awake();
        selectedCharacters = new List<Character>();
    }

    public int PlayerCount {
        get { return selectedCharacters.Count; }
    }

    public void SetCharcter(int playerNumber, Character character) {
        if(character == null)
            throw new ArgumentNullException("character");
        selectedCharacters[playerNumber] = character;
    }

    public void SetCharacters(IEnumerable<Character> characters) {
        if(characters == null)
            throw new ArgumentNullException("characters");
        selectedCharacters.Clear();
        selectedCharacters.AddRange(characters);
    }

    public Character SpawnCharacter(int playerNumber, Vector3 position) {
        Character runtimeCharacter = InstantiateCharacter(playerNumber);
        runtimeCharacter.transform.position = position;
        return runtimeCharacter;
    }

    protected Character GetCharacterPrefab(int playerNumber) {
        return selectedCharacters[playerNumber];
    }

    protected virtual Character InstantiateCharacter(int playerNumber) {
        return Instantiate(GetCharacterPrefab(playerNumber));
    }

}
