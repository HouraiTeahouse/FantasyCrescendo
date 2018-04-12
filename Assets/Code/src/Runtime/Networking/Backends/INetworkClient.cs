using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkClient : IDisposable {

  NetworkConnection Connection { get; }

  event Action<MatchConfig> OnMatchStarted;
  event Action<MatchResult> OnMatchFinished;

  event Action<MatchConfig> OnMatchConfigUpdated;

  event Action<uint, ArraySegment<MatchInput>> OnRecievedInputs;
  event Action<uint, MatchState, MatchInput?> OnRecievedState;
	event Action<bool> OnServerReady;
  event Action OnDisconnect;

  bool IsServerReady { get; }

  Task Connect(NetworkConnectionConfig config);
  void Disconnect();

  // Unreliable
  void SendInput(uint startTimestamp, byte validMask, IEnumerable<MatchInput> inputs);

  // Reliable
  void SetReady(bool isReady);

  // Reliable
  void SetConfig(PlayerConfig config);

  void Update();

}

}
