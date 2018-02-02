using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public enum PlatformHardness {
  Hard, Semisoft, Soft, Supersoft
}
    
public class Platform : RegisteredBehaviour<Platform, byte> {

  public IReadOnlyCollection<Collider> SolidColliders { get; private set; }
  public PlatformHardness Type;
  public Bounds[] CheckRegions;

  Collider[] solidColliders;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override void Awake() {
    base.Awake();
    var colliders = GetComponentsInChildren<Collider>();
    solidColliders = colliders.Where(c => !c.isTrigger).ToArray();
    SolidColliders = new ReadOnlyCollection<Collider>(solidColliders);
  }

  public static void CollisionStatusCheckAll(Collider collider) {
    foreach (var platform in Registry.Get<Platform>()) {
      if (platform == null) continue;
      platform.CollisionStatusCheck(collider);
    }
  }

  public bool CollisionStatusCheck(Collider collider) {
    var isIgnoring = IsIgnoringPlatform(collider);
    UpdateCollisionStatus(collider, isIgnoring);
    return isIgnoring;
  }

  public bool IsIgnoringPlatform(Collider collier) {
    var bounds = collier.bounds;
    var isTouching = false;
    foreach (var region in CheckRegions) {
      isTouching |= GetWorldRegion(region).Intersects(bounds);
    }
    return isTouching;
  }

  public void UpdateCollisionStatus(Collider collider, bool collide) {
    foreach (var platformCollider in solidColliders) {
      if (platformCollider.isTrigger) continue;
      Physics.IgnoreCollision(platformCollider, collider, collide);
    }
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    using (GizmoUtil.With(Color.magenta)) {
      foreach (var platformCollider in GetComponentsInChildren<Collider>()) {
        if (platformCollider.isTrigger) continue;
        GizmoUtil.DrawCollider(platformCollider);
      }
      if (CheckRegions == null) return;
      foreach (var region in CheckRegions) {
        GizmoUtil.DrawBox(GetWorldRegion(region));
      }
    }
  }

  Bounds GetWorldRegion(Bounds localRegion) {
    Bounds worldRegion = localRegion;
    worldRegion.center = transform.TransformPoint(worldRegion.center);
    worldRegion.size = Vector3.Scale(localRegion.size, transform.lossyScale);
    return worldRegion;
  }

}

}