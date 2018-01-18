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
    MatchHitboxSimulation.AddHitboxes(activeHitboxes);
    MatchHitboxSimulation.AddHurtboxes(Hurtboxes.Where(hurtbox => hurtbox.isActiveAndEnabled));
    return state;
  }

  public PlayerState ResetState(PlayerState state) => state;

}

public class MatchHitboxSimulation : IMatchSimulation {

  static List<Hitbox> ActiveHitboxes;
  static HashSet<Hurtbox> ActiveHurtboxes;
  static MatchHitboxSimulation() {
    ActiveHitboxes = new List<Hitbox>();
    ActiveHurtboxes = new HashSet<Hurtbox>();
  }

  Hurtbox[] HurtboxSet;

  public MatchHitboxSimulation() {
    HurtboxSet = new Hurtbox[256];
  }

  public static void AddHitboxes(IEnumerable<Hitbox> hitboxes) => ActiveHitboxes.AddRange(hitboxes);
  public static void AddHurtboxes(IEnumerable<Hurtbox> hitboxes) => ActiveHurtboxes.UnionWith(hitboxes);

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    ActiveHitboxes.Clear();
    return state;
  }

  IEnumerable<HitboxCollision> CreateCollisions() {
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