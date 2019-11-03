using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class AbstractHitDetector : MonoBehaviour, IComparable<AbstractHitDetector> {
  [NonSerialized]
  public uint PlayerID;
  public uint Priority;

  public int CompareTo(AbstractHitDetector hitDetector) {
    return Priority.CompareTo(hitDetector.Priority);
  }

}

public class Hurtbox : AbstractHitDetector {

  public HurtboxType Type = HurtboxType.Damageable;

  Collider[] colliders;

  public Vector3 Center {
    get {
      var center = Vector3.zero;
      if (colliders != null) {
        colliders = GetComponents<Collider>();
      }
      if (colliders.Length <= 0) {
        return transform.position;
      }
      foreach (var collider in colliders) {
        center += collider.bounds.center;
      }
      return center / colliders.Length;
    }
  }

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => Initalize();

  public void Initalize() {
    gameObject.layer = Config.Get<PhysicsConfig>().HurtboxLayer;
    colliders = GetComponents<Collider>();
    foreach (var collider in colliders) {
      collider.isTrigger = true;
    }
  }

#if UNITY_EDITOR
  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (isActiveAndEnabled) 
    using (GizmoUtility.With(HitboxUtil.GetHurtboxColor(Type))) {
      foreach (var collider in GetComponents<Collider>()) {
        if (!collider.enabled && collider.gameObject.activeInHierarchy) continue;
        GizmoUtility.DrawCollider(collider);
      }
    }
  }
#endif

}

}