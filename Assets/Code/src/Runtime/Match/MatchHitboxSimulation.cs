using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class PlayerCollisionManager {

  public readonly List<HitboxCollision>[] PlayerCollisions;

  public PlayerCollisionManager(MatchConfig config) {
    PlayerCollisions = new List<HitboxCollision>[config.PlayerCount];
    for (var i = 0; i < PlayerCollisions.Length; i++) {
      PlayerCollisions[i] = new List<HitboxCollision>();
    }
  }

  public void Add(HitboxCollision collision) {
    if (collision.IsSelfCollision) return;
    PlayerCollisions[collision.Destination.PlayerID].Add(collision);
  }

  public List<HitboxCollision> GetPlayerCollisions(uint id) {
    var collisions = PlayerCollisions[id];
    collisions.Sort();
    return collisions;
  }

  public void Clear() {
    foreach (var collisionSet in PlayerCollisions) {
      collisionSet.Clear();
    }
  }

}
    
public class MatchHitboxSimulation : IMatchSimulation {

  public static MatchHitboxSimulation Instance { get; set; }

  public readonly HashSet<Hitbox> ActiveHitboxes;
  public readonly HashSet<Hurtbox> ActiveHurtboxes;

  readonly MatchConfig Config;
  readonly PlayerCollisionManager CollisionManager;

  public MatchHitboxSimulation(MatchConfig config) {
    Instance = this;
    Config = config;
    ActiveHitboxes = new HashSet<Hitbox>();
    ActiveHurtboxes = new HashSet<Hurtbox>();
    CollisionManager = new PlayerCollisionManager(config);
  }

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    if (ActiveHitboxes.Count + ActiveHurtboxes.Count > 0) {
      Physics.SyncTransforms();
      CreateCollisions();
      for (var i = 0; i < Config.PlayerCount; i++) {
        var playerState = state.GetPlayerState(i);
        var collisions = CollisionManager.PlayerCollisions[i];
        ApplyCollisions(collisions, ref playerState, state);
        state.SetPlayerState(i,  playerState);
      }
    }
    CollisionManager.Clear();
    ActiveHitboxes.Clear();
    ActiveHurtboxes.Clear();
    return state;
  }

  // TODO(james7132): Split and generalize this... somehow.
  public void ApplyCollisions(List<HitboxCollision> collisions, ref PlayerState state, MatchState match) {
    if (collisions.Count <= 0 ) return;
    bool isShielded = false;
    bool isHit = false;
    foreach (var collision in collisions) {
      var source = collision.Source;
      int srcPlayerId = (int)collision.Source.PlayerID;
      int dstPlayerId = (int)collision.Destination.PlayerID;
      switch (collision.Destination.Type) {
        case HurtboxType.Damageable:
          var sourceState = match.GetPlayerState((int)source.PlayerID);

          // Check if hit is valid.
          if (isShielded || isHit || sourceState.HasHit(dstPlayerId)) continue;

          // Deal damage, knockback, hitlag, and hitstun
          state.Damage += source.BaseDamage;
          state.Velocity = source.GetKnocback(state.Damage, sourceState.Direction);
          state.Hitstun = source.GetHitstun(state.Damage);
          state.Hitlag = source.Hitlag;
          sourceState.Hitlag = source.Hitlag;

          // Mark the source as having hit the destination.
          sourceState.HitPlayer(dstPlayerId);
          match.SetPlayerState(srcPlayerId, sourceState);

          isHit = true;
          // TODO(james7132): Play Effect
          break;
        case HurtboxType.Shield:
          isShielded = true;
          state.ShieldDamage += (uint)(source.BaseDamage * 100);
          break;
        case HurtboxType.Invincible:
          // TODO(james7132): Play Effect
          break;
      }
    }
  }

  public void CreateCollisions() {
    var hurtboxes = ArrayPool<Hurtbox>.Shared.Rent(256);
    foreach (var hitbox in ActiveHitboxes) {
      var hurtboxCount = HitboxUtil.CollisionCheck(hitbox, hurtboxes);
      for (var i = 0; i < hurtboxCount; i++) {
        if (!ActiveHurtboxes.Contains(hurtboxes[i])) continue;
        CollisionManager.Add(new HitboxCollision {
          Source = hitbox,
          Destination = hurtboxes[i]
        });
      }
    }
    ArrayPool<Hurtbox>.Shared.Return(hurtboxes);
  }

  public Task Initialize(MatchConfig config) => Task.CompletedTask;
  public MatchState ResetState(MatchState state) => state;
  public void Dispose() {}

}

}