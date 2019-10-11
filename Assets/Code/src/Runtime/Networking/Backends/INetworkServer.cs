using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkServer : IDisposable {

  ICollection<NetworkClientPlayer> Clients { get; }

  // Signature: Client ID, Timestamp, Inputs
  event Action<int, uint, ArraySegment<MatchInput>> ReceivedInputs;

  event Action<NetworkClientPlayer> PlayerAdded;
  event Action<NetworkClientPlayer> PlayerUpdated;
  event Action<int> PlayerRemoved;

  Task Initialize();

  // Reliable
  void FinishMatch(MatchResult result);

	// Reliable
	void SetReady(bool ready);

  // Unreliable
  void BroadcastInput(uint startTimestamp, IEnumerable<MatchInput> input); 

  // Unreliable Sequenced
  void BroadcastState(uint timestamp, MatchState state, MatchInput? latestInput = null);

  void Update();

}

}
