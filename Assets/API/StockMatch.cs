using UnityEngine;
using Genso.API;

public class StockMatch : Match {

    [SerializeField]
    private CharacterData[] characters;

    private bool Added;

    void Start()  {
		Stage stage = Stage.Instance;
        Added = false;
        if (stage != null) {
            SetCharacters(characters);   
            StartMatch();
        }
    }

	protected override void OnSpawn (Character character) {
		GameObject characterObject = character.gameObject;
		characterObject.AddComponent<CharacterDeath>();
		characterObject.AddComponent<CharacterRespawn>();
		if(!Added) {
			characterObject.AddComponent<TestInput>();
			Added = true;
		}
	}

}
