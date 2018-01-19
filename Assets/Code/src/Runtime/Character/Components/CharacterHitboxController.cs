using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

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

  public void Presimulate(PlayerState state) { }

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    var activeHitboxes = from hitbox in Hitboxes 
                         where hitbox.isActiveAndEnabled && hitbox.Type != HitboxType.Inactive
                         select hitbox;
    MatchHitboxSimulation.Instance?.ActiveHitboxes.UnionWith(activeHitboxes);
    MatchHitboxSimulation.Instance?.ActiveHurtboxes.UnionWith(Hurtboxes.Where(hurtbox => hurtbox.isActiveAndEnabled));
    return state;
  }

  public PlayerState ResetState(PlayerState state) => state;

}

}