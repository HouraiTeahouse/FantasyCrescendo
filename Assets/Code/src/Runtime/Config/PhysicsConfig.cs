using HouraiTeahouse.Attributes;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Physics Config")]
public class PhysicsConfig : ScriptableObject {

  [Header("Movement")]
  public float GroundedSnapOffset = 0.5f;
  public float GroundedSnapDistance = 1.5f;

  [Header("Layers")]
  public LayerMask StageLayers;
  [Layer] public int HurtboxLayer;
  public int HurtboxLayerMask => 1 << HurtboxLayer;

  [Header("Knockback")]
  public float GlobalKnockbackScaling = 0.1f;

  [Header("Hitlag")]
  public uint BaseHitlag = 3;
  public uint HitlagScaling = 3;

  [Header("Gravity")]
  public float ShortJumpGravityMultiplier = 2f;
  public float DownwardGravityMultiplier = 2.5f;

}

}
