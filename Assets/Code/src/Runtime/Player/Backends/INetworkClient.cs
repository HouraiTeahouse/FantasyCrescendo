using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkClient : IDisposable {

  event Action<MatchConfig> OnMatchStarted;
  event Action<MatchResult> OnMatchFinished;

  event Action<MatchConfig> OnMatchConfigUpdated;

  event Action<uint, ArraySlice<MatchInput>> OnRecievedInputs;
  event Action<uint, MatchState> OnRecievedState;
	event Action<bool> OnServerReady;

  bool IsServerReady { get; }

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
