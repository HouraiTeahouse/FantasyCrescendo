using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public static class GizmoUtil {

  class GizmoDisposable : IDisposable {
    Matrix4x4 matrix;
    Color color;
    public GizmoDisposable() { color = Gizmos.color; }
    public void Dispose() => Gizmos.color = color;
  }

  public static IDisposable With(Color color) {
    Gizmos.color = color;
    return new GizmoDisposable();
  }

  public static IDisposable With(Matrix4x4 matrix) {
    Gizmos.matrix = matrix;
    return new GizmoDisposable();
  }

  public static IDisposable With(Transform transform) {
    Gizmos.matrix = transform.localToWorldMatrix;
    return new GizmoDisposable();
  }

  public static void DrawCollider(Collider collider, bool wire = true) {
    DrawBoxCollider(collider as BoxCollider, wire);
    DrawSphereCollider(collider as SphereCollider, wire);
    DrawCapsuleCollider(collider as CapsuleCollider, wire);
  }

  public static void DrawBoxCollider(BoxCollider collider, bool wire = true) {
    if (collider == null) return;
    using (With(collider.transform)) {
      DrawBox(collider.center, collider.size, wire);
    }
  }

  public static void DrawSphereCollider(SphereCollider collider, bool wire = true) {
    if (collider == null) return;
    var transform = collider.transform;
    var scale = transform.lossyScale;
    var maxComponent = Vector3.one * Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
    using (With(Matrix4x4.TRS(transform.position, transform.rotation, maxComponent))) {
      DrawSphere(collider.center, collider.radius, wire);
    }
  }

  public static void DrawCapsuleCollider(CapsuleCollider collider, bool wire = true) {
    if (collider == null) return;
    using (With(collider.transform)) {
      DrawCapsule(collider.center, collider.radius, collider.height, collider.direction, wire);
    }
  }

  public static void DrawBox(Bounds bounds, bool wire = true) => DrawBox(bounds.center, bounds.size, wire);

  public static void DrawBox(Vector3 center, Vector3 size, bool wire = true) {
    if (wire) { Gizmos.DrawWireCube(center, size); } 
    else { Gizmos.DrawCube(center, size); }
  }

  public static void DrawSphere(Vector3 center, float radius, bool wire = true) {
    if (wire) { Gizmos.DrawWireSphere(center, radius); } 
    else { Gizmos.DrawSphere(center, radius); }
  }

  static void DrawCapsule(Vector3 center, float radius, float height, int direction, bool wire = true)  {
    var diff = GetDiffVector(height, radius, direction);
    if (radius > height) {
      DrawSphere(center, radius, wire);
      return;
    }
    DrawSphere(center + diff, radius, wire);
    DrawSphere(center - diff, radius, wire);
    for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI / 2) {
      var lineDiff = GetLineDiff(i, radius, direction);
      var start = center + diff + lineDiff;
      var end = center - diff + lineDiff;
      Gizmos.DrawLine(start, end);
    }
  }

  static Vector3 GetDiffVector(float height, float radius, int direction) {
    var magnitude = height / 2 - radius;
    switch (direction) {
      case 0: return Vector3.right * magnitude;
      case 1: return Vector3.up * magnitude;
      default:
      case 2: return Vector3.forward * magnitude;
    }
  }

  static Vector3 GetLineDiff(float i, float radius, int direction) {
    var cos = Mathf.Cos(i);
    var sin = Mathf.Sin(i);
    switch (direction) {
      case 0: return new Vector3(0, cos, sin) * radius;
      case 1: return new Vector3(cos, 0, sin) * radius;
      default:
      case 2: return new Vector3(cos, sin, 0) * radius;
    }
  }

}

}

