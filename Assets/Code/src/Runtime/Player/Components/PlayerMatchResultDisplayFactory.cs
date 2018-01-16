using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerMatchResultDisplayFactory : PlayerViewFactory<PlayerMatchStats> {

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