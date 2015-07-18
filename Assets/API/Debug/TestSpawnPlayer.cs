using UnityEngine;
using Genso.API;

public class TestSpawnPlayer : Match {

    [SerializeField]
    private Character[] characters;

    void Start()  {
        Stage stage = FindObjectOfType<Stage>();
        if (stage != null) {
            SetCharacters(characters);   
            stage.StartMatch(this);
        }
    }

    protected override Character InstantiateCharacter(int playerNumber) {
        Character toSpawn = base.InstantiateCharacter(playerNumber);
        toSpawn.gameObject.AddComponent<CharacterDeath>();
        toSpawn.gameObject.AddComponent<CharacterRespawn>();
        toSpawn.gameObject.AddComponent<TestInput>();
        return toSpawn;
    }

}
