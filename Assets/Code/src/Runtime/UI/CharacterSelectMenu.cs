using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterSelectMenu : MonoBehaviour, IStateView<MatchConfig> {

  CharacterData[] Characters;
  PlayerSelectControls[] Players;

  public MainMenu MainMenu;
  public RectTransform CharacterView;
  public RectTransform Container;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    await DataLoader.LoadTask.Task;
    var characters = from character in Registry.Get<CharacterData>()
                     where character.IsSelectable && character.IsVisible
                     select character;
    if (!Debug.isDebugBuild) {
      characters = characters.Where(ch => !ch.IsDebug);
    }
    Characters = characters.ToArray();
    Players = Enumerable.Range(0, (int)GameMode.GlobalMaxPlayers).Select(CreateView).ToArray();
  }

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    if (MainMenu == null || MainMenu.CurrentGameMode == null) return;
    var maxPlayers = MainMenu.CurrentGameMode.MaxPlayers;
    for (byte i = 0; i < Players.Length; i++) {
      var playerConfig = new PlayerConfig();
      playerConfig.PlayerID = i;
      playerConfig.LocalPlayerID = (sbyte)i;
      playerConfig.Selection.CharacterID = Characters[0].Id;
      playerConfig.Selection.Pallete = i;
      Players[i].Config = playerConfig;
      Players[i].SetActive(false);
      Players[i].gameObject.SetActive(i < maxPlayers);
    }
  }

  public void ApplyState(MatchConfig config) {
    foreach (var player in config.PlayerConfigs) {
      Players[player.PlayerID].Config = player;
    }
  }

  public MatchConfig BuildMatchConfig(MatchConfig baseConfig) {
    baseConfig.PlayerConfigs = (from player in Players
                               where player.IsActive
                               select player.Config).ToArray();
    return baseConfig;
  }

  public uint NextCharacterID(uint currentId, bool backwards) {
    for (var i = 0; i < Characters.Length; i++) {
      if (Characters[i].Id != currentId) continue;
      var newIndex = (backwards ? i - 1 : i + 1) % Characters.Length;
      return Characters[newIndex].Id;
    }
    return Characters[0].Id;
  }

  public uint NextPallete(PlayerSelection selection, bool backwards) {
    var newPallete = (uint)(selection.Pallete + (backwards ? -1 : 1));
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    return newPallete % (uint)character.Portraits.Count;
  }

  PlayerSelectControls CreateView(int playerIndex) {
    var newObj = Instantiate(CharacterView);
    newObj.name = $"Player {playerIndex + 1} View";
    newObj.SetParent(Container, true);
    var playerSelectControls = newObj.GetComponentInChildren<PlayerSelectControls>();
    Assert.IsNotNull(playerSelectControls);
    playerSelectControls.PlayerID = (uint)playerIndex;
    playerSelectControls.CharacterSelectMenu = this;
    return playerSelectControls;
  }

}

}