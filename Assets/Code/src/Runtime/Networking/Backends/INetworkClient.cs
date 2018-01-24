using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

public interface INetworkClient : IDisposable {

  event Action<MatchConfig> OnMatchStarted;
  event Action<MatchResult> OnMatchFinished;

  event Action<uint, IEnumerable<MatchInput>> OnRecievedInputs;
  event Action<uint, MatchState> OnRecievedState;

  Task Connect(string ip, uint port);
  void Disconnect();

  // Unreliable
  void SendInput(uint startTimestamp, IEnumerable<MatchInput> inputs);

  // Reliable
  void SetReady(bool isReady);

  // Reliable
  void SetConfig(PlayerConfig config);

  void Update();

}

}
