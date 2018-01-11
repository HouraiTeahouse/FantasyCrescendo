using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkClient : IDisposable {

  event Action<uint, IEnumerable<GameInput>> ReceivedInputs;
  event Action<uint, GameState> ReceivedState;

  void SendInput(uint startTimestamp, IEnumerable<GameInput> inputs);

}

}
