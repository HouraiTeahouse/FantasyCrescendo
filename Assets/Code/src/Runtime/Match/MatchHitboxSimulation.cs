using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct PlayerHitboxCollisions : IComparable<PlayerHitboxCollisions> {
  public uint PlayerID;
  public IEnumerable<HitboxCollision> Collisions;

  public override bool Equals(object obj) {
    if (obj == null || GetType() != obj.GetType()) {
      return false;
    }
    var hitboxCollision = (PlayerHitboxCollisions)obj;
    return base.Equals (obj) && Collisions.SequenceEqual(hitboxCollision.Collisions);
  }
  
  public override int GetHashCode() => base.GetHashCode() ^ PlayerID.GetHashCode() ^ Collisions.GetHashCode();

  public int CompareTo(PlayerHitboxCollisions obj) => PlayerID.CompareTo(obj.PlayerID);
}
    
public class MatchHitboxSimulation : IMatchSimulation {

  public static MatchHitboxSimulation Instance { get; set; }

  public readonly HashSet<Hitbox> ActiveHitboxes;
  public readonly HashSet<Hurtbox> ActiveHurtboxes;

  public MatchHitboxSimulation() {
    Instance = this;
    ActiveHitboxes = new HashSet<Hitbox>();
    ActiveHurtboxes = new HashSet<Hurtbox>();
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    if (ActiveHitboxes.Count + ActiveHurtboxes.Count > 0) {
      Physics.SyncTransforms();
      var collisions = CreateCollisions();
      foreach (var playerCollisions in GetPlayerCollisions(collisions)) {
        ApplyCollisions(playerCollisions, ref state);
      }
      ActiveHitboxes.Clear();
      ActiveHurtboxes.Clear();
    }
    return state;
  }

  // TODO(james7132): Split and generalize this... somehow.
  public void ApplyCollisions(PlayerHitboxCollisions collisions, ref MatchState state) {
    var playerState = state.PlayerStates[collisions.PlayerID];
    bool isShielded = false;
    foreach (var collision in collisions.Collisions) {
      var source = collision.Source;
      switch (collision.Destination.Type) {
        case HurtboxType.Damageable:
          if (isShielded) continue;
          playerState.Damage += source.BaseDamage;
          playerState.Velocity = source.GetKnocback(playerState.Damage);
          playerState.Hitstun = source.GetHitstun(playerState.Damage);
          // TODO(james7132): Play Effect
          break;
        case HurtboxType.Shield:
          isShielded = true;
          break;
        case HurtboxType.Invincible:
          // TODO(james7132): Play Effect
          break;
      }
    }
    state.PlayerStates[collisions.PlayerID] = playerState;
  }

  public static IEnumerable<PlayerHitboxCollisions> GetPlayerCollisions(IEnumerable<HitboxCollision> collisions) {
    return from collision in collisions
           where !collision.IsSelfCollision
           group collision by collision.Destination.PlayerID into playerGroup
           orderby playerGroup.Key
           select new PlayerHitboxCollisions { 
             PlayerID = playerGroup.Key, 
             Collisions = playerGroup.OrderBy(c => c)
           };
  }

  public IEnumerable<HitboxCollision> CreateCollisions() {
    var hurtboxes = ArrayPool<Hurtbox>.Shared.Rent(256);
    foreach (var hitbox in ActiveHitboxes) {
      var hurtboxCount = HitboxUtil.CollisionCheck(hitbox, hurtboxes);
      for (var i = 0; i < hurtboxCount; i++) {
        if (!ActiveHurtboxes.Contains(hurtboxes[i])) continue;
        yield return new HitboxCollision { Source = hitbox, Destination = hurtboxes[i] };
      }
    }
    ArrayPool<Hurtbox>.Shared.Return(hurtboxes);
  }

  public Task Initialize(MatchConfig config) => Task.CompletedTask;
  public MatchState ResetState(MatchState state) => state;
  public void Dispose() {}

}

}