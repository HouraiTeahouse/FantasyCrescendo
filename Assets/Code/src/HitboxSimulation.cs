using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

public class HitboxSimulation {

  public static HitboxSimulation Current { get; set; }

  public static IDisposable Context(HitboxSimulation simulation) {
    return new SimulationContext(simulation);
  }

  private struct SimulationContext : IDisposable {

    readonly HitboxSimulation simulation;

    public SimulationContext(HitboxSimulation simulation) {
      this.simulation = simulation;
      HitboxSimulation.Current = simulation;
    }

    public void Dispose() {
      if (simulation == null) {
        return;
      }
      simulation.Clear();
      HitboxSimulation.Current = null;
    }

  }

  public int Count { get; private set; }

  readonly IEnumerable<Tuple<HitboxType, HitboxType>> resolutionOrder;
  readonly Dictionary<HitboxType, List<HitboxEntry>> entries;

  public HitboxSimulation(IEnumerable<Tuple<HitboxType, HitboxType>> resolveOrder) {
    resolutionOrder = resolveOrder;
    entries = new Dictionary<HitboxType, List<HitboxEntry>>();
    foreach (var pair in resolutionOrder) {
      entries[pair.Item1] = new List<HitboxEntry>();
      entries[pair.Item2] = new List<HitboxEntry>();
    }
  }

  public void RegisterEntry(HitboxEntry entry) {
    List<HitboxEntry> typeEntries;
    if (entries.TryGetValue(entry.Type, out typeEntries)) {
      typeEntries.Add(entry);
      Count++;
    }
  }

  public IEnumerable<HitboxCollision> Resolve() {
    if (Count <= 0) {
      return Enumerable.Empty<HitboxCollision>();
    }
    return resolutionOrder.SelectMany(p => {
      var collisions = HitboxUtil.CreateCollisions(entries[p.Item1], entries[p.Item2]);
      return collisions.OrderByDescending(c => c.Priority);
    });
  }

  public void Clear() {
    if (Count <= 0) {
      return;
    }
    foreach (var entrySet in entries.Values) {
      entrySet.Clear();
    }
    Count = 0;
  }

}

}
