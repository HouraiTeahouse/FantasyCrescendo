using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;  

public class HurtboxBuilder {

  public readonly Hurtbox Hurtbox;

  public HurtboxBuilder(Hurtbox box) {
    Hurtbox = box;
  }

  public HurtboxBuilder AsPlayer(uint playerID) {
    Hurtbox.PlayerID = playerID;
    return this;
  }

  public HurtboxBuilder WithPosition(Vector3 position) {
    Hurtbox.transform.position = position;
    return this;
  }

  public HurtboxBuilder WithType(HurtboxType type) {
    Hurtbox.Type = type;
    return this;
  }

  public HurtboxBuilder WithRadius(float radius) {
    Hurtbox.GetComponent<SphereCollider>().radius = radius;
    return this;
  }
  
  public HurtboxBuilder WithEnabled(bool enabled) {
    Hurtbox.enabled = enabled;
    return this;
  }
  
  public HurtboxBuilder WithActive(bool active) {
    Hurtbox.gameObject.SetActive(active);
    return this;
  }

  public HurtboxBuilder WithPriority(uint priority) {
    Hurtbox.Priority = priority;
    return this;
  }

  public Hurtbox Build() => Hurtbox;

}
