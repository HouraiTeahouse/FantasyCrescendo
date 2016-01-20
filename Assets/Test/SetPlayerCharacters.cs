using UnityEngine;
using System.Collections;
using Hourai.SmashBrew;
using UnityEngine.EventSystems;

public class SetPlayerCharacters : MonoBehaviour, ISubmitHandler {

    [SerializeField]
    private CharacterData character;

    public void OnSubmit(BaseEventData eventData) {
        foreach (var player in SmashGame.Players) {
            player.Character = character;
            player.Pallete = 0;
            if(player.PlayerNumber < 2)
                player.Type = Player.PlayerType.HumanPlayer;
            else
                player.Type = Player.PlayerType.None;
        }
    }

}
