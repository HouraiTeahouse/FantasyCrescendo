using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerDisplayFactory : PlayerViewFactory {

  public RectTransform Container;

  public override void OnCreateView(PlayerConfig config, GameObject view) {
    var viewTransform = view.transform as RectTransform;
    if  (viewTransform == null) {
      return;
    }
    viewTransform.SetParent(Container, false);
  }

}

}
