using HouraiTeahouse.EditorAttributes;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Physics Config")]
public class PhysicsConfig : ScriptableObject {

  public LayerMask StageLayers;
  [Layer] public int HurtboxLayer;

  public float GroundedSnapOffset = 0.5f;
  public float GroundedSnapDistance = 1.5f;

  public int HurtboxLayerMask => 1 << HurtboxLayer;

  public float GlobalKnockbackScaling = 0.1f;

  public uint BaseHitlag = 3;
  public uint HitlagScaling = 3;

}

}
