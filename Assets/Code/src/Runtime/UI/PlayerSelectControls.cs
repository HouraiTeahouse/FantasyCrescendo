using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSelectControls : MonoBehaviour {

  PlayerConfig Config;

  public uint PlayerID;
  public CharacterSelectMenu CharacterSelectMenu;

  void UpdatePlayerInfo() {
    if (CharacterSelectMenu == null) return;
    CharacterSelectMenu.UpdatePlayer(PlayerID, Config);
  }

  public void CycleCharacter(bool backward) {
    if (CharacterSelectMenu == null) return;
    Config.Selection.CharacterID = CharacterSelectMenu.NextCharacterID(Config.Selection.CharacterID, backward);
    UpdatePlayerInfo();
  }

  public void CycleColor(bool backward) {
    if (CharacterSelectMenu == null) return;
    Config.Selection.Pallete = (byte)CharacterSelectMenu.NextPallete(Config.Selection, backward);
    UpdatePlayerInfo();
  }

}

}