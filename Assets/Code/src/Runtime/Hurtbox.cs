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

  public HurtboxType Type;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() => Initalize();

  public void Initalize() {
    gameObject.layer = Config.Get<PhysicsConfig>().HurtboxLayer;
    foreach (var collider in GetComponents<Collider>()) {
      collider.isTrigger = true;
    }
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (isActiveAndEnabled) 
    using (GizmoUtil.With(HitboxUtil.GetHurtboxColor(Type))) {
      foreach (var collider in GetComponents<Collider>()) {
        if (!collider.enabled && collider.gameObject.activeInHierarchy) continue;
        GizmoUtil.DrawCollider(collider);
      }
    }
  }

}

}