using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class ConstantRotation : MonoBehaviour {

  public Vector3 RotationPerSecond;

  /// <summary>
  /// Update is called every frame, if the MonoBehaviour is enabled.
  /// </summary>
  void Update() {
    transform.Rotate(RotationPerSecond * Time.deltaTime);
  }

}

}
