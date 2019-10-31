using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public struct ConnectionStats : IMergable<ConnectionStats> {
  public int TotalBytesOut;
  public int IncomingPacketsCount;
  public int IncomingPacketsLost;
  public int CurrentRTT;

  public float PacketLossPercent {
    get {
      var total = IncomingPacketsCount += IncomingPacketsLost;
      if (total == 0) return 0;
      return IncomingPacketsLost / total;
    }
  }

  public ConnectionStats MergeWith(ConnectionStats other) {
    TotalBytesOut += other.TotalBytesOut;
    IncomingPacketsCount += other.IncomingPacketsCount;
    IncomingPacketsLost += other.IncomingPacketsLost;
    CurrentRTT = Mathf.Max(CurrentRTT, other.CurrentRTT);
    return this;
  }

}

}