using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkClient : IDisposable {

  event Action<uint, IEnumerable<GameInput>> RecievedInputs;
  event Action<uint, GameState> RecievedState;

  void SendInput(uint startTimestamp, IEnumerable<GameInput> inputs);

}

}
