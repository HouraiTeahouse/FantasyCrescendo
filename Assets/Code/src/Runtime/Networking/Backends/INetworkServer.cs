using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkServer : IDisposable {

  int ClientCount { get; }

  // Signature: Client ID, Timestamp, Inputs
  event Action<int, uint, IEnumerable<MatchInput>> ReceivedInputs;

  void BroadcastInput(uint startTimestamp, IEnumerable<MatchInput> input);
  void BroadcastState(uint timestamp, MatchState state);

}

}
