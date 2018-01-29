using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterSelectMenu : MonoBehaviour, IStateView<MatchConfig> {

  CharacterData[] Characters;
  PlayerSelectControls[] players;

  public ReadOnlyCollection<PlayerSelectControls> Players { get; private set; }

  public MainMenu MainMenu;
  public RectTransform CharacterView;
  public RectTransform Container;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    await DataLoader.LoadTask.Task;
    Debug.Log($"Total Registered Characters: {Registry.Get<CharacterData>().Count}");
    var characters = new List<CharacterData>();
    foreach (var character in Registry.Get<CharacterData>()) {
      var selectable = character.IsSelectable && character.IsVisible;
      if (!Debug.isDebugBuild) selectable &= !character.IsDebug;
      Debug.Log($"Charcter: {character}. Selectable: {selectable}");
      if (selectable) {
        characters.Add(character);
      }
    }
    Characters = characters.ToArray();
    Debug.Log($"Total Selectable Characters: {Characters.Length}");
    players = Enumerable.Range(0, (int)GameMode.GlobalMaxPlayers).Select(CreateView).ToArray();
    Players = new ReadOnlyCollection<PlayerSelectControls>(players);
  }

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    if (MainMenu == null || MainMenu.CurrentGameMode == null) return;
    var maxPlayers = MainMenu.CurrentGameMode.MaxPlayers;
    for (byte i = 0; i < players.Length; i++) {
      var playerConfig = new PlayerConfig();
      playerConfig.PlayerID = i;
      playerConfig.LocalPlayerID = (sbyte)i;
      playerConfig.Selection.CharacterID = Characters[0].Id;
      playerConfig.Selection.Pallete = i;
      players[i].Config = playerConfig;
      players[i].SetActive(false);
      players[i].gameObject.SetActive(i < maxPlayers);
    }
  }

  public void ApplyState(MatchConfig config) {
    foreach (var player in players) {
      player.IsActive = false;
    }
    foreach (var player in config.PlayerConfigs) {
      players[player.PlayerID].IsActive = true;
      players[player.PlayerID].Config = player;
    }
  }

  public MatchConfig BuildMatchConfig(MatchConfig baseConfig) {
    baseConfig.PlayerConfigs = (from player in players
                               where player.IsActive
                               select player.Config).ToArray();
    return baseConfig;
  }

  public PlayerSelection CreateNewSelection(byte playerId) {
    var character = Characters.First();
    var baseSelection = new PlayerSelection {
      CharacterID = character.Id,
      Pallete = playerId
    };
    return GetNextAvailableSelection(baseSelection, character.Portraits.Count);
  }

  public uint NextCharacterID(uint currentId, bool backwards) {
    for (var i = 0; i < Characters.Length; i++) {
      if (Characters[i].Id != currentId) continue;
      var newIndex = (backwards ? i - 1 : i + 1) % Characters.Length;
      return Characters[newIndex].Id;
    }
    return Characters[0].Id;
  }

  public PlayerSelection NextPallete(PlayerSelection selection, byte playerId, bool backwards) {
    var character = Registry.Get<CharacterData>().Get(selection.CharacterID);
    if (character == null) {
      selection.Pallete = (byte)(((backwards ? -1 : 1) + selection.Pallete) % GameMode.GlobalMaxPlayers);
      return selection;
    }
    return GetNextAvailableSelection(selection, character.Portraits.Count, backwards);
  }

  PlayerSelection GetNextAvailableSelection(PlayerSelection selection, int limit, bool backwards = false) {
    int diff = backwards ? -1 : 1;
    for (var i = 1; i < limit; i++) {
      var newSelection = selection;
      newSelection.Pallete = (byte)((selection.Pallete + i * diff) % limit);
      bool present = false;
      for (byte j = 0; j < players.Length; j++) {
        if (!players[j].IsActive) continue;
        Debug.LogWarning($"{newSelection} {players[j].Config.Selection}");
        present |= players[j].Config.Selection.Equals(newSelection);
      }
      if (!present) {
        Debug.Log($"{newSelection.Pallete} {present}");
        return newSelection;
      }
    }
    throw new InvalidOperationException("No available palletes remaining");
  }

  PlayerSelectControls CreateView(int playerIndex) {
    var newObj = Instantiate(CharacterView);
    newObj.name = $"Player {playerIndex + 1} View";
    newObj.SetParent(Container, true);
    var playerSelectControls = newObj.GetComponentInChildren<PlayerSelectControls>();
    Assert.IsNotNull(playerSelectControls);
    playerSelectControls.PlayerID = (byte)playerIndex;
    playerSelectControls.CharacterSelectMenu = this;
    return playerSelectControls;
  }

}

}