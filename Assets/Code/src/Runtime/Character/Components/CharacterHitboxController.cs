using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterHitboxController : MonoBehaviour, IPlayerSimulation {

  Hitbox[] Hitboxes;
  Hurtbox[] Hurtboxes;

  public Task Initialize(PlayerConfig config, bool isView = false) {
    var hitDetectors = GetComponentsInChildren<AbstractHitDetector>();
    Hitboxes = hitDetectors.OfType<Hitbox>().ToArray();
    Hurtboxes = hitDetectors.OfType<Hurtbox>().ToArray();
    foreach (var hitDetector in hitDetectors) {
      hitDetector.PlayerID = config.PlayerID;
    }
    return Task.CompletedTask;
  }

  public void Presimulate(PlayerState state) { 
    foreach (var hitbox in Hitboxes) {
      hitbox.Presimulate();
    }
  }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    var matchHitboxes = MatchHitboxSimulation.Instance?.ActiveHitboxes;
    var matchHurtboxes = MatchHitboxSimulation.Instance?.ActiveHurtboxes;
    if (matchHitboxes != null) {
      foreach (var hitbox in Hitboxes) {
        if (!hitbox.IsActive) continue;
        matchHitboxes.Add(hitbox);
      }
    }
    if (matchHurtboxes != null) {
      foreach (var hurtbox in Hurtboxes) {
        if (!hurtbox.isActiveAndEnabled) continue;
        matchHurtboxes.Add(hurtbox);
      }
    }
    return state;
  }

  public PlayerState ResetState(PlayerState state) => state;

}

}