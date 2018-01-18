using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public class DrawColliders : MonoBehaviour {

  public Color GizmoColor;
  public bool Wire;

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    using (GizmoUtil.With(GizmoColor)) {
      foreach (var collider in GetComponentsInChildren<Collider>()) {
        GizmoUtil.DrawCollider(collider, Wire);
      }
    }
  }

}

}