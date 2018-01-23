using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkClient : IDisposable {

  event Action<uint, IEnumerable<MatchInput>> ReceivedInputs;
  event Action<uint, MatchState> ReceivedState;

  Task Connect(string ip, uint port);

  void SendInput(uint startTimestamp, IEnumerable<MatchInput> inputs);

}

}
