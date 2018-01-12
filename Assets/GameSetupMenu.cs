using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSetupMenu : MonoBehaviour {

  [Serializable]
  public class PlayerSelectionMenu {
    public Toggle ActiveToggle;
    public Dropdown CharacterDropdwon;
    public Dropdown ColorDropdown;
  }

  public Dropdown StageDropdwon;
  public int ColorCount;
  public PlayerSelectionMenu[] PlayerMenus;
  public GameConfig Config;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start() {
    var characters = Registry.Get<CharacterData>().Where(c => c.IsSelectable && c.IsVisible).ToArray();
    var stages = Registry.Get<SceneData>().Where(scene => scene.IsSelectable && scene.IsVisible && scene.IsStage).ToArray();

    var characterOptionData = characters.Select(character => 
      new Dropdown.OptionData { text = character.LongName }
    ).ToList();

    var colorOptionData = Enumerable.Range(0, ColorCount).Select(color => 
      new Dropdown.OptionData { text = (color + 1).ToString() }
    ).ToList();

    StageDropdwon.options = stages.Select(scene => 
      new Dropdown.OptionData { text = scene.Name }
    ).ToList();

    Config.PlayerConfigs = new PlayerConfig[PlayerMenus.Length];
    for (uint i = 0; i < PlayerMenus.Length; i++) {
      var player = PlayerMenus[i];
      player.CharacterDropdwon.options = characterOptionData;
      player.ColorDropdown.options = colorOptionData;
      
      // Do this to avoid bad capture semantics
      uint playerIndex = i;

      player.CharacterDropdwon.onValueChanged.AddListener(index => {
        Config.PlayerConfigs[playerIndex].Selection.CharacterID = characters[i].Id;
      });
      player.ColorDropdown.onValueChanged.AddListener(index => {
        Config.PlayerConfigs[playerIndex].Selection.Pallete = (uint)index;
      });

      Config.PlayerConfigs[i].PlayerID = i;
      Config.PlayerConfigs[i].LocalPlayerID = i;
      Config.PlayerConfigs[i].Selection.CharacterID = characters.First().Id;
      Config.PlayerConfigs[i].Selection.Pallete = 0;
    }

    StageDropdwon.onValueChanged.AddListener(index => {
      Config.StageID = stages[index].Id;
    });
    Config.StageID = stages.First().Id;
  }

  public void LaunchGame() {
    var playerConfigs = new List<PlayerConfig>();
    for (int i = 0; i < PlayerMenus.Length; i++) {
      if (!PlayerMenus[i].ActiveToggle.isOn) continue;
      playerConfigs.Add(Config.PlayerConfigs[i]);
    }
    Config.PlayerConfigs = playerConfigs.ToArray();

    //TODO (james7132): Load game properly
    Debug.Log("GAME START");
  }

}

}
