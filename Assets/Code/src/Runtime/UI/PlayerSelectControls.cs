using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerSelectControls : MonoBehaviour {

  [SerializeField]
  PlayerConfig config;

  public byte PlayerID;
  public bool IsActive;
  public PlayerConfig Config {
    get { return config; }
    set { 
      bool changed = !config.Selection.Equals(value.Selection);
      config = value;
      config.PlayerID = PlayerID;
      if (changed && IsActive && Config.IsLocal) {
        PlayerUpdated?.Invoke(PlayerID, Config);
      }
      UpdatePlayerInfo();
    }
  }

  public event Action<byte, PlayerConfig> PlayerUpdated;

  public CharacterSelectMenu CharacterSelectMenu;

  public Button[] Buttons;
  public List<Button> LocalOnlyButtons;
  public Object[] ActiveComponents;

  bool isLocal;
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
      var interactable = Config.IsLocal;
      if (LocalOnlyButtons.Contains(button)) {
        interactable &= IsActive;
      }
      button.interactable = interactable;
    }
    foreach (var obj in ActiveComponents) {
      ObjectUtil.SetActive(obj, IsActive);
    }
  }

  public void CycleCharacter(bool backward) {
    if (CharacterSelectMenu == null) return;
    var newId = CharacterSelectMenu.NextCharacterID(Config.Selection.CharacterID, backward);
    bool changed = Config.Selection.CharacterID != newId;
    config.Selection.CharacterID = newId;
    if (changed) {
      PlayerUpdated?.Invoke(PlayerID, config);
    }
    UpdatePlayerInfo();
  }

  public void CycleColor(bool backward) {
    if (CharacterSelectMenu == null) return;
    var newSelection = CharacterSelectMenu.NextPallete(Config.Selection, PlayerID, backward);
    bool changed = !Config.Selection.Equals(newSelection);
    config.Selection = newSelection;
    Debug.LogError(newSelection);
    if (changed) {
      PlayerUpdated?.Invoke(PlayerID, config);
    }
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