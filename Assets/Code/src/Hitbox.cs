using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class Hitbox : MonoBehaviour {

  public HitboxEntry Data;

  public float Radius = 0.5f;
  public Vector3 Offset;

  public Vector3 GetCenter() {
    return transform.TransformPoint(Offset);
  }

  void OnDrawGizmos() {
    if (!isActiveAndEnabled) {
      return;
    }
    var oldColor = Gizmos.color;
    Gizmos.color = HitboxUtil.GetHitboxColor(Data.Type);
    Gizmos.DrawWireSphere(GetCenter(), Radius);
    Gizmos.color = oldColor;
  }

}

}
