using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace HouraiTeahouse.FantasyCrescendo {

public enum MovementType {
  Normal, DirectionalInfluenceOnly, Locked
}

[Serializable]
public class CharacterStateData {
  [Tooltip("Corresponding timeline controller")]
  public TimelineAsset Timeline;
  [Tooltip("Corresponding animation for the state")]
  public AnimationClip AnimationClip;
  [Tooltip("Length of time the state lasts")]
  public float Length;
  [Tooltip("Minimum movement speeds. Interpolated based on input magnitude.")]
  public float MinMoveSpeed;
  [Tooltip("Maxiumum movement speeds. Interpolated based on input magnitude.")]
  public float MaxMoveSpeed;
  public float RotationOffset;
  public StateEntryPolicy EntryPolicy = StateEntryPolicy.Normal;
  public MovementType MovementType = MovementType.Normal;
  public bool CanTurn = true;
  public float KnockbackResistance;
}

}

