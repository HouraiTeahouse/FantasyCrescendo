using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public struct PlayerHitboxCollisions {
  public uint PlayerID;
  public IEnumerable<HitboxCollision> Collisions;
}
    
public class MatchHitboxSimulation : IMatchSimulation {

  public static MatchHitboxSimulation Instance { get; set; }

  Hurtbox[] HurtboxSet;
  public readonly HashSet<Hitbox> ActiveHitboxes;
  public readonly HashSet<Hurtbox> ActiveHurtboxes;

  public MatchHitboxSimulation() {
    Instance = this;
    HurtboxSet = new Hurtbox[256];
    ActiveHitboxes = new HashSet<Hitbox>();
    ActiveHurtboxes = new HashSet<Hurtbox>();
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    var collisions = CreateCollisions();
    foreach (var playerCollisions in GetPlayerCollisions(collisions)) {
      var prioritizedCollisions = PrioritizeCollisions(playerCollisions.Collisions);

    }
    Clear();
    return state;
  }

  void Clear() {
    ActiveHitboxes.Clear();
    ActiveHurtboxes.Clear();
  }

  public static IEnumerable<PlayerHitboxCollisions> GetPlayerCollisions(IEnumerable<HitboxCollision> collisions) {
    return from collision in collisions
           where !collision.IsSelfCollision
           group collision by collision.Destination.PlayerID into playerGroup
           orderby playerGroup.Key
           select new PlayerHitboxCollisions { PlayerID = playerGroup.Key, Collisions = playerGroup };
  }

  public static IEnumerable<HitboxCollision> PrioritizeCollisions(IEnumerable<HitboxCollision> collisions) {
    return from collision in collisions
           orderby collision.Destination.Type, collision.Priority descending
           select collision;
  }

  public IEnumerable<HitboxCollision> CreateCollisions() {
    foreach (var hitbox in ActiveHitboxes) {
      var hurtboxCount = hitbox.CollisionCheck(HurtboxSet);
      for (var i = 0; i < hurtboxCount; i++) {
        if (!ActiveHurtboxes.Contains(HurtboxSet[i])) continue;
        yield return new HitboxCollision { Source = hitbox, Destination = HurtboxSet[i] };
      }
    }
  }

  public Task Initialize(MatchConfig config) => Task.CompletedTask;
  public MatchState ResetState(MatchState state) => state;
  public void Dispose() {}

}

}