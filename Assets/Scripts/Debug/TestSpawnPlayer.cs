using UnityEngine;

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

}
