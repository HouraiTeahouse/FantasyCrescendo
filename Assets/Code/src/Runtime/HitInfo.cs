using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct HitInfo {
  public Hitbox Source;
  public Hurtbox Destination;
  public MatchState MatchState;

  public float DamageDealt;
  public Vector2 KnockbackDealt;

  public Vector3 Position => Destination.Center;
}

}