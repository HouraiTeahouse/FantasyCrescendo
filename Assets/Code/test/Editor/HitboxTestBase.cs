using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;

public abstract class HitboxTestBase : GameObjectTest {

  protected HitboxBuilder CreateHitbox() {
    var builder = new HitboxBuilder(CreateObject<Hitbox>());
    builder.Hitbox.Type = HitboxType.Offensive;
    return builder;
  }

  protected HurtboxBuilder CreateHurtbox() {
    var builder = new HurtboxBuilder(CreateObject<Hurtbox>());
    builder.Hurtbox.gameObject.AddComponent<SphereCollider>();
    builder.Hurtbox.Initalize();
    return builder;
  }

}