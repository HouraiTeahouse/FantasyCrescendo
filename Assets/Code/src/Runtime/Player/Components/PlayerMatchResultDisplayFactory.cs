using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerMatchResultDisplayFactory : PlayerViewFactory<PlayerMatchStats> {

  public RectTransform Container;

  protected override void Preinitialize(PlayerConfig config, GameObject view) {
    var viewTransform = view.transform as RectTransform;
    if  (viewTransform == null) {
      return;
    }
    viewTransform.SetParent(Container, false);
  }

}

}