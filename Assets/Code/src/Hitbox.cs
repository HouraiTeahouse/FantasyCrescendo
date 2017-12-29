using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class Hitbox : MonoBehaviour {

  public HitboxType Type;

  public float Radius = 0.5f;
  public Vector3 Offset;

  public Vector3 GetCenter() {
    return transform.TransformPoint(Offset);
  }

  //public IEnumerable<HitboxCollision>

  void OnDrawGizmos() {
    if (!isActiveAndEnabled) {
      return;
    }
    var oldColor = Gizmos.color;
    Gizmos.color = HitboxUtil.GetHitboxColor(Type);
    Gizmos.DrawWireSphere(GetCenter(), Radius);
    Gizmos.color = oldColor;
  }

}

}
