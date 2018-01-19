using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

public class Hitbox : AbstractHitDetector {

  static Hitbox() {
    const int kCacheSize = 256;
    CollisionSet = new Collider[kCacheSize];
    DedupCheck = new HashSet<Hurtbox>();
  }

  static Collider[] CollisionSet;
  static HashSet<Hurtbox> DedupCheck;

  public HitboxType Type;

  public Vector3 Offset;
  public float Radius = 0.5f;

  public bool IsActive => isActiveAndEnabled && Type != HitboxType.Inactive;
  public Vector3 Center => transform.TransformPoint(Offset);

  bool IsValid(Hurtbox hurtbox) => hurtbox != null && DedupCheck.Add(hurtbox) && hurtbox.isActiveAndEnabled;
  int CheckColliders() {
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
    return Physics.OverlapSphereNonAlloc(Center, Radius, CollisionSet, layerMask, QueryTriggerInteraction.Collide);
  }

  public int CollisionCheck(Hurtbox[] hurtboxSet) {
    var collisionCount = CheckColliders();
    int hurtboxCount = 0;
    DedupCheck.Clear();
    for (int i = 0; i < collisionCount && hurtboxCount < hurtboxSet.Length; i++) {
      Assert.IsNotNull(CollisionSet[i]);
      var hurtbox = CollisionSet[i].GetComponent<Hurtbox>();
      if (IsValid(hurtbox)) {
        hurtboxSet[hurtboxCount++] = hurtbox;
      }
    }
    return hurtboxCount;
  }

#if UNITY_EDITOR
  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (EditorApplication.isPlayingOrWillChangePlaymode && !IsActive) { return; }
    Gizmos.color = HitboxUtil.GetHitboxColor(Type);
    Gizmos.DrawWireSphere(Center, Radius);
  }
#endif 

}

}
