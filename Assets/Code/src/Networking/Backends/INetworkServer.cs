using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkServer : IDisposable {

  int ClientCount { get; }

  // Signature: Client ID, Timestamp, Inputs
  event Action<int, uint, IEnumerable<GameInput>> ReceivedInputs;

  void BroadcastInput(uint startTimestamp, IEnumerable<GameInput> input);
  void BroadcastState(uint timestamp, GameState state);

}

}
