using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct HitboxCollision {
  public HitboxEntry Source;
  public HitboxEntry Destination;

  public int Priority {
    get { return Source.Priority; }
  }

}

}
