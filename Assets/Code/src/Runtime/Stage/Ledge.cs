using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CapsuleCollider))]
public sealed class Ledge : RegisteredBehaviour<Ledge, byte> {

  public bool Direction;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override void Awake() {
    base.Awake();
    foreach (var collider in GetComponents<Collider>()) {
      collider.isTrigger = true;
    }
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    var capsuleCollider = GetComponent<CapsuleCollider>();
    if (capsuleCollider == null) return;
    Gizmos.color = Color.red;
    var center = capsuleCollider.center;
    var diff = GetDiffVector(capsuleCollider);
    if (capsuleCollider.radius > capsuleCollider.height) {
      Gizmos.DrawWireSphere(transform.TransformPoint(center), capsuleCollider.radius);
      return;
    }
    Gizmos.DrawWireSphere(transform.TransformPoint(center + diff), capsuleCollider.radius);
    Gizmos.DrawWireSphere(transform.TransformPoint(center - diff), capsuleCollider.radius);
    for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI / 2) {
      var lineDiff = GetLineDiff(i, capsuleCollider);
      var start = center + diff + lineDiff;
      var end = center - diff + lineDiff;
      Gizmos.DrawLine(transform.TransformPoint(start), transform.TransformPoint(end));
    }
  }

  Vector3 GetDiffVector(CapsuleCollider collider) {
    var magnitude = collider.height / 2 - collider.radius;
    switch (collider.direction) {
      case 0: return Vector3.right * magnitude;
      case 1: return Vector3.up * magnitude;
      default:
      case 2: return Vector3.forward * magnitude;
    }
  }

  Vector3 GetLineDiff(float i, CapsuleCollider collider) {
    var cos = Mathf.Cos(i);
    var sin = Mathf.Sin(i);
    switch (collider.direction) {
      case 0: return new Vector3(0, cos, sin) * collider.radius;
      case 1: return new Vector3(cos, 0, sin) * collider.radius;
      default:
      case 2: return new Vector3(cos, sin, 0) * collider.radius;
    }
  }

  /// <summary>
  /// Reset is called when the user hits the Reset button in the Inspector's
  /// context menu or when adding the component the first time.
  /// </summary>
  void Reset() {
    Id = (byte)new Random().Next();
    // TODO(james7132): Add this to some config
    gameObject.tag = "Ledge";
  }

}

}