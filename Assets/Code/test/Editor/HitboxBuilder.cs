using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;  

public class HitboxBuilder {

  public readonly Hitbox Hitbox;

  public HitboxBuilder(Hitbox box) {
    Hitbox = box;
  }

  public HitboxBuilder AsPlayer(uint playerID) {
    Hitbox.PlayerID = playerID;
    return this;
  }

  public HitboxBuilder WithOffset(Vector3 offset) {
    Hitbox.Offset = offset;
    return this;
  }

  public HitboxBuilder WithPosition(Vector3 position) {
    Hitbox.transform.position = position;
    return this;
  }

  public HitboxBuilder WithType(HitboxType type) {
    Hitbox.Type = type;
    return this;
  }

  public HitboxBuilder WithRadius(float radius) {
    Hitbox.Radius = radius;
    return this;
  }
  
  public HitboxBuilder WithEnabled(bool enabled) {
    Hitbox.enabled = enabled;
    return this;
  }
  
  public HitboxBuilder WithActive(bool active) {
    Hitbox.gameObject.SetActive(active);
    return this;
  }

  public HitboxBuilder WithPriority(uint priority) {
    Hitbox.Priority = priority;
    return this;
  }

  public Hitbox Build() => Hitbox;

}
