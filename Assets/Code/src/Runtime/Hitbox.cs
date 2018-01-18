using System;
using System.Collections.Generic;
using UnityEngine;
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

  public int CollisionCheck(Hurtbox[] hurtboxSet) {
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
    var collisionCount = Physics.OverlapSphereNonAlloc(transform.TransformPoint(Offset), Radius, 
                                                       CollisionSet, layerMask, QueryTriggerInteraction.Collide);
    int hurtboxCount = 0;
    DedupCheck.Clear();
    for (int i = 0; i < collisionCount && hurtboxCount < hurtboxSet.Length; i++) {
      var hurtbox = CollisionSet[i].GetComponentInParent<Hurtbox>();
      if (DedupCheck.Add(hurtbox) && hurtbox.isActiveAndEnabled) {
        hurtboxSet[hurtboxCount++] = hurtbox;
      }
    }
    Array.Sort<AbstractHitDetector>(hurtboxSet, 0, hurtboxCount);
    return hurtboxCount;
  }

#if UNITY_EDITOR
  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (EditorApplication.isPlayingOrWillChangePlaymode && !IsActive) { return; }
    Gizmos.color = HitboxUtil.GetHitboxColor(Type);
    Gizmos.DrawWireSphere(transform.TransformPoint(Offset), Radius);
  }
#endif 

}

}
