using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

public class GameSetupMenu : MonoBehaviour {

  public Dropdown StageDropdwon;
  public Button[] ValidationUIElements;
  public MainMenu MainMenu;
  public CharacterSelectMenu CharacterSelectMenu;

  public MatchConfig Config;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  async void Start() {
    await DataLoader.LoadTask.Task;

    Debug.Log("Building Setup Menu");

    var stages = Registry.Get<SceneData>().Where(scene => scene.IsSelectable && scene.IsVisible && scene.Type == SceneType.Stage).ToArray();

    StageDropdwon.options = stages.Select(scene => 
      new Dropdown.OptionData { text = scene.Name }
    ).ToList();

    StageDropdwon.onValueChanged.AddListener(index => {
      Config.StageID = stages[index].Id;
      UpdateValidattion();
    });
    Config.StageID = stages.FirstOrDefault()?.Id ?? 0;

    UpdateValidattion();
  }

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() => UpdateValidattion();

  void UpdateValidattion() {
    if (MainMenu.CurrentGameMode == null || ValidationUIElements.Length <= 0) return; 
    if (CharacterSelectMenu != null) {
      Config = CharacterSelectMenu.BuildMatchConfig(Config);
    }
    var isValid = MainMenu.CurrentGameMode.IsValidConfig(Config);
    foreach (var button in ValidationUIElements) {
      if (button == null) continue;
      button.interactable = isValid;
    }
  }

  public async void LaunchGame() {
    await MainMenu.CurrentGameMode.Execute(Config);
  }

}

}
