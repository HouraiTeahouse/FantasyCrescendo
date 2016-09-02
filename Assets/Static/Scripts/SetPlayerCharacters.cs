using HouraiTeahouse.SmashBrew;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetPlayerCharacters : MonoBehaviour, ISubmitHandler {

    [SerializeField]
    CharacterData character;

    public void OnSubmit(BaseEventData eventData) {
        foreach (Player player in Player.ActivePlayers) {
            player.Selection = new PlayerSelection {Character = character, Pallete = 0};
            player.Type = player.ID < 2 ? PlayerType.HumanPlayer : PlayerType.None;
        }
    }

}