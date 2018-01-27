using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterSelectMenu : MonoBehaviour, IStateView<MatchConfig> {

  GenericView<PlayerConfig>[] Players;

  public MainMenu MainMenu;
  public RectTransform CharacterView;
  public RectTransform Container;
  public MatchConfig Config;

  CharacterData[] Characters;

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
    var playerConfigs = new PlayerConfig[MainMenu.CurrentGameMode.MaxPlayers];
    for (byte i = 0; i < playerConfigs.Length; i++) {
      playerConfigs[i].PlayerID = i;
      playerConfigs[i].LocalPlayerID = (sbyte)i;
      playerConfigs[i].Selection.CharacterID = Characters[0].Id;
      playerConfigs[i].Selection.Pallete = i;
    }
    Config.PlayerConfigs = playerConfigs;
    ApplyState(Config);
  }

  public void UpdatePlayer(uint playerId, PlayerConfig config) {
    Config.PlayerConfigs[playerId % Config.PlayerConfigs.Length] = config;
    ApplyState(Config);
  }

  public void ApplyState(MatchConfig config) {
    var playerConfigs = config.PlayerConfigs;
    if (playerConfigs == null || Players == null) return;
    for (var i = 0; i < Players.Length; i++) {
      var active = playerConfigs.Any(p => p.PlayerID == i);
      Players[i].SetActive(active);
      if (!active) continue;
      var playerConfig = playerConfigs.First(p => p.PlayerID == i);
      Players[i].ApplyState(playerConfig);
    }
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

  GenericView<PlayerConfig> CreateView(int playerIndex) {
    var newObj = Instantiate(CharacterView);
    newObj.name = $"Player {playerIndex + 1} View";
    newObj.SetParent(Container, true);
    var playerSelectControls = newObj.GetComponentInChildren<PlayerSelectControls>();
    if (playerSelectControls != null) {
      playerSelectControls.PlayerID = (uint)playerIndex;
      playerSelectControls.CharacterSelectMenu = this;
    }
    var view = new GenericView<PlayerConfig>(newObj.gameObject);
    if (Config.PlayerConfigs != null && playerIndex < Config.PlayerConfigs.Length) {
      view.ApplyState(Config.PlayerConfigs[playerIndex]);
    }
    return view;
  }

}

public class GenericView<T> : IStateView<T> {

  public readonly GameObject GameObject;
  readonly IStateView<T>[] Views;

  public GenericView(GameObject viewObj) {
    GameObject = viewObj;
    Views = viewObj.GetComponentsInChildren<IStateView<T>>();
  }

  public void SetActive(bool active)  => ObjectUtil.SetActive(GameObject, active); 

  public void ApplyState(T state) => Views.ApplyState(state);

}

}