using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkClient : IDisposable {

  event Action<uint, IEnumerable<MatchInput>> ReceivedInputs;
  event Action<uint, MatchState> ReceivedState;

  void SendInput(uint startTimestamp, IEnumerable<MatchInput> inputs);

}

}
