using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StatTracker {
  public double Value { get; protected set; }
  public virtual void Update(double value) => Value = value;
  public void Reset() => Value = 0f;
}

public class CounterStatTracker : StatTracker {
  public override void Update(double value) => Value += value;
}

public class RollingAverageStatTracker : StatTracker {
  public const double kRetentionRate = 0.1f;
  public override void Update(double value) => Value += (value - Value) * kRetentionRate;
}

public static class GameStats {

  public static readonly NetworkingStats Network = new NetworkingStats();

  public class NetworkingStats {
    public readonly ClientStats Client = new ClientStats();
  }

  public class ClientStats {
    public readonly StatTracker UnconfirmedInputs = new RollingAverageStatTracker();
  }

}