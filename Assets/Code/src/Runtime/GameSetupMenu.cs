using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSetupMenu : MonoBehaviour {

  [Serializable]
  public class PlayerSelectionMenu {
    public Toggle ActiveToggle;
    public Dropdown CharacterDropdwon;
    public Dropdown ColorDropdown;
  }

  public Dropdown StageDropdwon;
  public Dropdown GameModeDropdown;
  public Button[] ValidationUIElements;
  public int ColorCount;
  public PlayerSelectionMenu[] PlayerMenus;
  public MatchConfig Config;

  GameMode GameMode;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  async void Start() {
    await DataLoader.LoadTask.Task;

    Debug.Log("Building Setup Menu");

    var characters = Registry.Get<CharacterData>().Where(c => c.IsSelectable && c.IsVisible).ToArray();
    var stages = Registry.Get<SceneData>().Where(scene => scene.IsSelectable && scene.IsVisible && scene.Type == SceneType.Stage).ToArray();
    var gameModes = Registry.Get<GameMode>().Where(c => c.IsSelectable && c.IsVisible).ToArray();

    var characterOptionData = characters.Select(character => 
      new Dropdown.OptionData { text = character.LongName }
    ).ToList();

    var colorOptionData = Enumerable.Range(0, ColorCount).Select(color => 
      new Dropdown.OptionData { text = (color + 1).ToString() }
    ).ToList();

    StageDropdwon.options = stages.Select(scene => 
      new Dropdown.OptionData { text = scene.Name }
    ).ToList();

    GameModeDropdown.options = gameModes.Select(mode => 
      new Dropdown.OptionData { text = mode.GetType().Name.Replace("GameMode", "") }
    ).ToList();

    Config.PlayerConfigs = new PlayerConfig[PlayerMenus.Length];
    for (byte i = 0; i < PlayerMenus.Length; i++) {
      var player = PlayerMenus[i];
      player.CharacterDropdwon.options = characterOptionData;
      player.ColorDropdown.options = colorOptionData;
      
      // Do this to avoid bad capture semantics
      uint playerIndex = i;

      player.ActiveToggle.onValueChanged.AddListener(index => UpdateValidattion());
      player.CharacterDropdwon.onValueChanged.AddListener(index => {
        Config.PlayerConfigs[playerIndex].Selection.CharacterID = characters[i].Id;
        UpdateValidattion();
      });
      player.ColorDropdown.onValueChanged.AddListener(index => {
        Config.PlayerConfigs[playerIndex].Selection.Pallete = (byte)index;
        UpdateValidattion();
      });
      var pallete = (byte)Mathf.Clamp(i, 0, GameMode.GlobalMaxPlayers);
      player.ColorDropdown.value = pallete;

      Config.PlayerConfigs[i].PlayerID = i;
      Config.PlayerConfigs[i].LocalPlayerID = i;
      Config.PlayerConfigs[i].Selection.CharacterID = characters.FirstOrDefault()?.Id ?? 0;
      Config.PlayerConfigs[i].Selection.Pallete = pallete;
    }

    StageDropdwon.onValueChanged.AddListener(index => {
      Config.StageID = stages[index].Id;
      UpdateValidattion();
    });
    GameModeDropdown.onValueChanged.AddListener(index => {
      GameMode = gameModes[index];
      UpdateValidattion();
    });
    Config.StageID = stages.FirstOrDefault()?.Id ?? 0;
    GameMode = gameModes.FirstOrDefault();

    UpdateValidattion();
  }

  MatchConfig UpdateConfig() {
    var config = Config;
    var playerConfigs = new List<PlayerConfig>();
    for (int i = 0; i < PlayerMenus.Length; i++) {
      if (!PlayerMenus[i].ActiveToggle.isOn) continue;
      playerConfigs.Add(Config.PlayerConfigs[i]);
    }
    config.PlayerConfigs = playerConfigs.ToArray();
    return config;
  }

  void UpdateValidattion() {
    if (GameMode == null || ValidationUIElements.Length <= 0) return; 
    var isValid = GameMode.IsValidConfig(UpdateConfig());
    foreach (var button in ValidationUIElements) {
      if (button == null) continue;
      button.interactable = isValid;
    }
  }

  public async void LaunchGame() {
    await GameMode.Execute(UpdateConfig());
  }

}

}
