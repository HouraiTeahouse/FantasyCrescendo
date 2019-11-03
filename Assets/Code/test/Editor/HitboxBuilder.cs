using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;  

public class HitboxBuilder {

  public Hitbox Hitbox;

  public HitboxBuilder() {
      Hitbox = new Hitbox();
  }

  public HitboxBuilder AsPlayer(int playerID) {
    Hitbox.PlayerId = playerID;
    return this;
  }

  public HitboxBuilder WithKnockback(LinearScaledValue knockback) {
    Hitbox.Info.Knockback = knockback;
    return this;
  }

  public HitboxBuilder WithKnockbackAngle(float knockbackAngle) {
    Hitbox.Info.KnockbackAngle = knockbackAngle;
    return this;
  }

  public HitboxBuilder WithHitstun(LinearScaledValue hitstun) {
    Hitbox.Info.Hitstun = hitstun;
    return this;
  }

  public HitboxBuilder WithOffset(Vector3 offset) {
    Hitbox.Info.Offset = offset;
    return this;
  }

  public HitboxBuilder WithMirrorDirection(bool mirror) {
    Hitbox.Info.MirrorDirection = mirror;
    return this;
  }

  public HitboxBuilder WithPosition(Vector3 position) {
    Hitbox.Position = position;
    return this;
  }

  public HitboxBuilder WithType(HitboxType type) {
    Hitbox.Info.Type = type;
    return this;
  }

  public HitboxBuilder WithRadius(float radius) {
    Hitbox.Info.Radius = radius;
    return this;
  }

  public HitboxBuilder WithPriority(uint priority) {
    Hitbox.Info.Priority = priority;
    return this;
  }

  public Hitbox Build() => Hitbox;

}
