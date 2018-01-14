using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo {

public class PlayerIndicatorFactory : MonoBehaviour {

  public static PlayerIndicatorFactory Instance;

  public RectTransform Prefab;
  public RectTransform Container;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => Instance = this;

  public RectTransform CreateIndicator() {
    Assert.IsNotNull(Prefab);
    var indicator = Instantiate(Prefab);
    indicator.SetParent(Container, false);
    return indicator;
  }

}

}
