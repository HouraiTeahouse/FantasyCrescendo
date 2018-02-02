using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches ;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections;

public class MatchHitboxSimulationTest : HitboxTestBase {

  // MatchHitboxSimulation CreateSimulation(Hitbox[] hitboxes, Hurtbox[] hurtboxes) {
  //   var simulation = new MatchHitboxSimulation();
  //   simulation.ActiveHitboxes.UnionWith(hitboxes);
  //   simulation.ActiveHurtboxes.UnionWith(hurtboxes);
  //   return simulation;
  // }

	// [Test]
	// public void CreateCollisions_returns_empty_without_active_hitboxes() {
  //   var simulation = CreateSimulation(
  //     new Hitbox[] {
  //     }, new Hurtbox[] {
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 1).Build(),
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 2).Build(),
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 3).Build(),
  //     });
  //   CollectionAssert.AreEqual(Enumerable.Empty<HitboxCollision>(),
  //                             simulation.CreateCollisions());
	// }

	// [Test]
	// public void CreateCollisions_returns_empty_without_active_hurtboxes() {
  //   var simulation = CreateSimulation(
  //     new Hitbox[] {
  //       CreateHitbox().WithOffset(Vector3.one * 1).WithRadius(1).Build(),
  //       CreateHitbox().WithOffset(Vector3.one * 2).WithRadius(1).Build(),
  //       CreateHitbox().WithOffset(Vector3.one * 3).WithRadius(1).Build(),
  //     }, new Hurtbox[] {
  //     });
  //   CollectionAssert.AreEqual(Enumerable.Empty<HitboxCollision>(),
  //                             simulation.CreateCollisions());
	// }

	// [Test]
	// public void CreateCollisions_returns_valid_collisions() {
  //   var hitbox = CreateHitbox().WithOffset(Vector3.one * 2).WithRadius(5).Build();
  //   var hurtboxes = new Hurtbox[] {
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 1).Build(),
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 2).Build(),
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 3).Build(),
  //   };
  //   var simulation = CreateSimulation( new Hitbox[] { hitbox }, hurtboxes);
  //   var results = hurtboxes.Select(hurt => new HitboxCollision {
  //     Source = hitbox, Destination = hurt
  //   });
  //   CollectionAssert.AreEquivalent(results, simulation.CreateCollisions());
	// }

	// [Test]
	// public void CreateCollisions_handles_multiple_collisions() {
  //   var hitboxes = new Hitbox[] {
  //     CreateHitbox().WithOffset(Vector3.one * 1).WithRadius(5).Build(),
  //     CreateHitbox().WithOffset(Vector3.one * 2).WithRadius(5).Build(),
  //     CreateHitbox().WithOffset(Vector3.one * 3).WithRadius(5).Build(),
  //   };
  //   var hurtboxes = new Hurtbox[] {
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 1).Build(),
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 2).Build(),
  //     CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 3).Build(),
  //   };
  //   var simulation = CreateSimulation(hitboxes, hurtboxes);
  //   var results = from hitbox in hitboxes
  //                 from hurtbox in hurtboxes
  //                 select new HitboxCollision { Source = hitbox, Destination = hurtbox };
  //   CollectionAssert.AreEquivalent(results, simulation.CreateCollisions());
	// }

	// [Test]
	// public void GetPlayerCollisions_ignores_self_collisions() {
  //   var hitboxes = new HitboxCollision [] {
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(1).Build(),
  //       Destination = CreateHurtbox().AsPlayer(1).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(2).Build(),
  //       Destination = CreateHurtbox().AsPlayer(2).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(3).Build(),
  //       Destination = CreateHurtbox().AsPlayer(3).Build()
  //     },
  //   };
  //   CollectionAssert.AreEquivalent(Enumerable.Empty<PlayerHitboxCollisions>(), 
  //                                  MatchHitboxSimulation.GetPlayerCollisions(hitboxes));
	// }

	// [Test]
	// public void GetPlayerCollisions_orders_by_destination_player_id() {
  //   var hitboxes = new HitboxCollision [] {
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(3).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(1).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(2).Build()
  //     },
  //   };
  //   Assert.That(MatchHitboxSimulation.GetPlayerCollisions(hitboxes), Is.Ordered);
	// }

	// [Test]
	// public void GetPlayerCollisions_is_internally_ordered() {
  //   var hitboxes = new HitboxCollision [] {
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(3).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(1).Build()
  //     },
  //     new HitboxCollision {
  //       Source = CreateHitbox().AsPlayer(0).Build(),
  //       Destination = CreateHurtbox().AsPlayer(2).Build()
  //     },
  //   };
  //   Assert.That(MatchHitboxSimulation.GetPlayerCollisions(hitboxes).Select(h => h.Collisions), Is.All.Ordered);
	// }

	// [Test]
	// public void Simulate_clears_active_hit_detectors() {
  //   var simulation = CreateSimulation(
  //     new Hitbox[] {
  //       CreateHitbox().WithOffset(Vector3.one * 1).WithRadius(1).Build(),
  //       CreateHitbox().WithOffset(Vector3.one * 2).WithRadius(1).Build(),
  //       CreateHitbox().WithOffset(Vector3.one * 3).WithRadius(1).Build(),
  //     }, new Hurtbox[] {
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 1).Build(),
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 2).Build(),
  //       CreateHurtbox().WithRadius(1).WithPosition(Vector3.one * 3).Build(),
  //     });
  //   var config = new MatchConfig { PlayerConfigs = new PlayerConfig[4] };
  //   simulation.Simulate(new MatchState(config), new MatchInputContext(new MatchInput(config)));
  //   Assert.AreEqual(0, simulation.ActiveHitboxes.Count);
  //   Assert.AreEqual(0, simulation.ActiveHurtboxes.Count);
	// }

}
