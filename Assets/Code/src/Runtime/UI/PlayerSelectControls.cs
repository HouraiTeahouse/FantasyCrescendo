using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSelectControls : MonoBehaviour {

  [SerializeField]
  PlayerConfig config;

  public uint PlayerID;
  public bool IsActive;
  public PlayerConfig Config {
    get { return config; }
    set { 
      config = value;
      UpdatePlayerInfo();
    }
  }

  public CharacterSelectMenu CharacterSelectMenu;

  public Button[] Buttons;
  public Object[] ActiveComponents;

  IStateView<PlayerConfig>[] Views;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Views = GetComponentsInChildren<IStateView<PlayerConfig>>();
  }

  void UpdatePlayerInfo() { 
    Views.ApplyState(Config); 
    foreach (var button in Buttons) {
      button.interactable = Config.IsLocal;
    }
    foreach (var obj in ActiveComponents) {
      ObjectUtil.SetActive(obj, IsActive);
    }
  }

  public void CycleCharacter(bool backward) {
    if (CharacterSelectMenu == null) return;
    config.Selection.CharacterID = CharacterSelectMenu.NextCharacterID(Config.Selection.CharacterID, backward);
    UpdatePlayerInfo();
  }

  public void CycleColor(bool backward) {
    if (CharacterSelectMenu == null) return;
    config.Selection.Pallete = (byte)CharacterSelectMenu.NextPallete(Config.Selection, backward);
    UpdatePlayerInfo();
  }

  public void SetActive(bool active) {
    IsActive = active;
    UpdatePlayerInfo();
  }

  public void ToggleActive() {
    IsActive = !IsActive;
    UpdatePlayerInfo();
  }

}

}