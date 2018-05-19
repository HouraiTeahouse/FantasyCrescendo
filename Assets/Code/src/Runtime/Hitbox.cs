using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

public class Hitbox : AbstractHitDetector {

  Vector3? oldCenter_;

  public Vector3 OldCenter;

  public HitboxType Type;

  public Vector3 Offset;
  public float Radius = 0.5f;

  public float BaseDamage = 10f;
  public float BaseKnockback = 5f;
  [Range(-180f, 180f)] public float KnockbackAngle = 45f;
  public float KnockbackScaling = 1f;
  public uint BaseHitstun = 1;
  public float HitstunScaling = 0.1f;
  public bool MirrorDirection = true;

  public bool IsActive => isActiveAndEnabled && Type != HitboxType.Inactive;
  public Vector3 Center => transform.TransformPoint(Offset);

  public uint Hitlag {
    get {
      var config = Config.Get<PhysicsConfig>();
      var absDamage = Mathf.Abs(BaseDamage);
      return (uint)Mathf.FloorToInt(absDamage / config.HitlagScaling + config.BaseHitlag); 
    }
  }

  float KnockbackAngleRad => Mathf.Deg2Rad * KnockbackAngle;

  public Vector2 GetKnockbackDirection(bool direction) {
    var dirMult = 1f;
    if (MirrorDirection && !direction) {
      dirMult = -1;
    }
    return new Vector2(dirMult * Mathf.Cos(KnockbackAngleRad), Mathf.Sin(KnockbackAngleRad));
  }

  public float GetKnockbackScale(float damage) {
    return Config.Get<PhysicsConfig>().GlobalKnockbackScaling * Mathf.Max(0, BaseKnockback + KnockbackScaling * damage);
  }

  public Vector2 GetKnocback(float damage, bool dir) => GetKnockbackScale(damage) * GetKnockbackDirection(dir);

  public uint GetHitstun(float damage) => (uint)Mathf.Max(0, BaseHitstun + Mathf.FloorToInt(HitstunScaling * damage));

  public void Presimulate() {
    oldCenter_ = Center;
  }

  public int GetCollidedHurtboxes(Hurtbox[] hurtboxes) {
    HitboxUtil.FlushHurtboxDedupCache();
    var arrayPool = ArrayPool<Collider>.Shared;
    var colliders = arrayPool.Rent(hurtboxes.Length);
    var colliderCount = FullCollisionCheck(colliders);
    int hurtboxCount = 0;
    for (int i = 0; i < colliderCount && hurtboxCount < hurtboxes.Length; i++) {
      Assert.IsNotNull(colliders[i]);
      var hurtbox = colliders[i].GetComponent<Hurtbox>();
      if (HitboxUtil.IsValidHurtbox(hurtbox)) {
        hurtboxes[hurtboxCount++] = hurtbox;
      }
    }
    arrayPool.Return(colliders);
    return hurtboxCount;
  }

  int StaticCollisionCheck(Collider[] colliders) {
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;
    return Physics.OverlapSphereNonAlloc(Center, Radius, colliders, layerMask, QueryTriggerInteraction.Collide);
  }

  int InterpolatedCollisionCheck(Collider[] colliders) {
    if (!oldCenter_.HasValue) return 0;
    var arrayPool = ArrayPool<RaycastHit>.Shared;

    var hits = arrayPool.Rent(colliders.Length);
    var diff = Center - oldCenter_.Value;
    var distance = diff.magnitude;
    var direction = diff.normalized;
    var layerMask = Config.Get<PhysicsConfig>().HurtboxLayerMask;

    var count = Physics.SphereCastNonAlloc(Center, Radius, direction, hits, distance, layerMask, QueryTriggerInteraction.Collide);
    for (var i = 0; i < count; i++) {
      colliders[i] = hits[i].collider;
    }

    arrayPool.Return(hits);

    return count;
  }

  int FullCollisionCheck(Collider[] colliders) {
    var arrayPool = ArrayPool<Collider>.Shared;
    var staticResults = arrayPool.Rent(colliders.Length);
    var interpolatedResults = arrayPool.Rent(colliders.Length);

    var staticCount = StaticCollisionCheck(staticResults);
    var interpolatedCount = InterpolatedCollisionCheck(interpolatedResults);

    Array.Copy(staticResults, colliders, staticCount);
    Array.Copy(interpolatedResults, 0, colliders, staticCount, Math.Min(interpolatedCount, colliders.Length - staticCount));

    arrayPool.Return(staticResults);
    arrayPool.Return(interpolatedResults);

    return ArrayUtil.RemoveDuplicates(colliders);
  }

#if UNITY_EDITOR
  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (EditorApplication.isPlayingOrWillChangePlaymode && !IsActive) return;
    Gizmos.color = HitboxUtil.GetHitboxColor(Type);
    if (oldCenter_.HasValue) {
      GizmoUtil.DrawCapsule(Center, OldCenter, Radius);
    } else {
      Gizmos.DrawWireSphere(Center, Radius);
    }
  }
#endif 

}

}
