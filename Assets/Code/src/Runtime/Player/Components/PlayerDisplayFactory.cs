using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Players {

public class PlayerDisplayFactory : PlayerViewFactory<PlayerState> {

  public RectTransform Container;

  RectTransform[] views;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    views = new RectTransform[GameMode.GlobalMaxPlayers];
  }

  protected override void Preinitialize(PlayerConfig config, GameObject view) {
    var viewTransform = view.transform as RectTransform;
    if (viewTransform == null) return;
    viewTransform.SetParent(Container, false);
    
    // As all of the character views are loaded asynchronously. Some players
    // may load in out ofo order. This forcibly reorders all children to assure that 
    // the display is ordered properly by Player IDs.
    var playerId = config.PlayerID;
    if (playerId < 0 || playerId > views.Length) return;
    Assert.IsNull(views[playerId]);
    views[playerId] = viewTransform;
    foreach (var subView in views) {
      if (subView == null) continue;
      subView.SetAsLastSibling();
    }
  }

}

}
