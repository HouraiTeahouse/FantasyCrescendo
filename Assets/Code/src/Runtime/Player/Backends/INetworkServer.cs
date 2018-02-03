using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkServer : IDisposable {

  ICollection<NetworkClientPlayer> Clients { get; }

  // Signature: Client ID, Timestamp, Inputs
  event Action<uint, uint, ArraySlice<MatchInput>> ReceivedInputs;

  event Action<NetworkClientPlayer> PlayerAdded;
  event Action<NetworkClientPlayer> PlayerUpdated;
  event Action<uint> PlayerRemoved;

  // Reliable
  void FinishMatch(MatchResult result);

	// Reliable
	void SetReady(bool ready);

  // Unreliable
  void BroadcastInput(uint startTimestamp, byte validMask, IEnumerable<MatchInput> input); 

  // Unreliable Sequenced
  void BroadcastState(uint timestamp, MatchState state);

  void Update();

}

}
