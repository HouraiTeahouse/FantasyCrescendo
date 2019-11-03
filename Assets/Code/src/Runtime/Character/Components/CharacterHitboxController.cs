using HouraiTeahouse.FantasyCrescendo.Players;
using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterHitboxController : PlayerComponent {

  Hitbox[] Hitboxes;
  Hurtbox[] Hurtboxes;

  public override Task Initialize(PlayerConfig config, bool isView = false) {
    var hitDetectors = GetComponentsInChildren<AbstractHitDetector>();
    Hitboxes = hitDetectors.OfType<Hitbox>().ToArray();
    Hurtboxes = hitDetectors.OfType<Hurtbox>().ToArray();
    foreach (var hitDetector in hitDetectors) {
      hitDetector.PlayerID = config.PlayerID;
    }
    return base.Initialize(config, isView);
  }

  public override void Presimulate(in PlayerState state) { 
  }

  public override void Simulate(ref PlayerState state, in PlayerInputContext input) {
    var matchHitboxes = MatchHitboxSimulation.Instance?.ActiveHitboxes;
    var matchHurtboxes = MatchHitboxSimulation.Instance?.ActiveHurtboxes;
    if (matchHitboxes != null) {
      foreach (var hitbox in Hitboxes) {
        if (!hitbox.Info.IsActive) continue;
        matchHitboxes.Add(hitbox);
      }
    }
    if (matchHurtboxes != null) {
      foreach (var hurtbox in Hurtboxes) {
        if (!hurtbox.isActiveAndEnabled) continue;
        matchHurtboxes.Add(hurtbox);
      }
    }
  }

}

}
