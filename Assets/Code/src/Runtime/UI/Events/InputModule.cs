using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.FantasyCrescendo {

public class InputModule: StandaloneInputModule {

  public BaseInput InputInterface;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override void Awake() {
    m_InputOverride = InputInterface;
    base.Awake();
  }

}

}