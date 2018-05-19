using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {
    
public static class GizmoUtil {

  class GizmoDisposable : IDisposable {
    Matrix4x4 matrix;
    Color color;

    public GizmoDisposable() { 
      matrix = Gizmos.matrix;
      color = Gizmos.color; 
    }

    public void Dispose() {
      Gizmos.matrix = matrix;
      Gizmos.color = color;
    }

  }

  public static IDisposable With(Color color) {
    var disposable = new GizmoDisposable();
    Gizmos.color = color;
    return disposable; 
  }

  public static IDisposable With(Matrix4x4 matrix) {
    var disposable = new GizmoDisposable();
    Gizmos.matrix = matrix;
    return disposable;
  }

  public static IDisposable With(Transform transform) => With(transform.localToWorldMatrix);

  public static void DrawCollider(Collider collider, bool wire = true) {
    DrawBoxCollider(collider as BoxCollider, wire);
    DrawSphereCollider(collider as SphereCollider, wire);
    DrawCapsuleCollider(collider as CapsuleCollider);
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

  public static void DrawCapsuleCollider(CapsuleCollider collider) {
    if (collider == null) return;
    using (With(collider.transform)) {
      var diff = GetDiffVector(collider.height, collider.radius, collider.direction);
      DrawCapsule(collider.center + diff, collider.center - diff, collider.radius);
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

  static void DrawCapsule(Vector3 center, float radius, float height, int direction)  {
  }

  public static void DrawCapsule(Vector3 start, Vector3 end, float radius) {
		Vector3 up = (end-start).normalized*radius;
		Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
		Vector3 right = Vector3.Cross(up, forward).normalized*radius;
		
		float height = (start-end).magnitude;
		float sideLength = Mathf.Max(0, (height*0.5f)-radius);
		Vector3 middle = (end+start)*0.5f;
		
		start = middle+((start-middle).normalized*sideLength);
		end = middle+((end-middle).normalized*sideLength);
		
		//Radial circles
		DrawCircle(start, up, radius);	
		DrawCircle(end, -up, radius);
		
		//Side lines
		Gizmos.DrawLine(start+right, end+right);
		Gizmos.DrawLine(start-right, end-right);
		
		Gizmos.DrawLine(start+forward, end+forward);
		Gizmos.DrawLine(start-forward, end-forward);
		
		for(int i = 1; i < 26; i++){
			//Start endcap
			Gizmos.DrawLine(Vector3.Slerp(right, -up, i/25.0f)+start, Vector3.Slerp(right, -up, (i-1)/25.0f)+start);
			Gizmos.DrawLine(Vector3.Slerp(-right, -up, i/25.0f)+start, Vector3.Slerp(-right, -up, (i-1)/25.0f)+start);
			Gizmos.DrawLine(Vector3.Slerp(forward, -up, i/25.0f)+start, Vector3.Slerp(forward, -up, (i-1)/25.0f)+start);
			Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i/25.0f)+start, Vector3.Slerp(-forward, -up, (i-1)/25.0f)+start);
			
			//End endcap
			Gizmos.DrawLine(Vector3.Slerp(right, up, i/25.0f)+end, Vector3.Slerp(right, up, (i-1)/25.0f)+end);
			Gizmos.DrawLine(Vector3.Slerp(-right, up, i/25.0f)+end, Vector3.Slerp(-right, up, (i-1)/25.0f)+end);
			Gizmos.DrawLine(Vector3.Slerp(forward, up, i/25.0f)+end, Vector3.Slerp(forward, up, (i-1)/25.0f)+end);
			Gizmos.DrawLine(Vector3.Slerp(-forward, up, i/25.0f)+end, Vector3.Slerp(-forward, up, (i-1)/25.0f)+end);
		}
  }

	public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f) {
		Vector3 _up = up.normalized * radius;
		Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
		Vector3 _right = Vector3.Cross(_up, _forward).normalized*radius;
		
		Matrix4x4 matrix = new Matrix4x4();
		
		matrix[0] = _right.x;
		matrix[1] = _right.y;
		matrix[2] = _right.z;
		
		matrix[4] = _up.x;
		matrix[5] = _up.y;
		matrix[6] = _up.z;
		
		matrix[8] = _forward.x;
		matrix[9] = _forward.y;
		matrix[10] = _forward.z;
		
		Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
		Vector3 _nextPoint = Vector3.zero;
		
		for(var i = 0; i < 91; i++){
			_nextPoint.x = Mathf.Cos((i*4)*Mathf.Deg2Rad);
			_nextPoint.z = Mathf.Sin((i*4)*Mathf.Deg2Rad);
			_nextPoint.y = 0;
			
			_nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);
			
			Gizmos.DrawLine(_lastPoint, _nextPoint);
			_lastPoint = _nextPoint;
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

