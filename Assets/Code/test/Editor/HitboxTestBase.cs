using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;

public abstract class HitboxTestBase : GameObjectTest {

  protected HitboxBuilder CreateHitbox() {
    return new HitboxBuilder().WithType(HitboxType.Offensive);
  }

  protected HurtboxBuilder CreateHurtbox() {
    var builder = new HurtboxBuilder(CreateObject<Hurtbox>());
    builder.Hurtbox.gameObject.AddComponent<SphereCollider>();
    builder.Hurtbox.Initalize();
    return builder;
  }

}
