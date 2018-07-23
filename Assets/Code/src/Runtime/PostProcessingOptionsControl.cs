using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class PostProcessingOptionsControl : MonoBehaviour {

  public PostProcessLayer PostProcessLayer;
  public Option Antialiasing;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (PostProcessLayer == null) return;
    if (Antialiasing != null) {
      PostProcessLayer.antialiasingMode = Antialiasing.Get<PostProcessLayer.Antialiasing>();
    }
  }

}

}
