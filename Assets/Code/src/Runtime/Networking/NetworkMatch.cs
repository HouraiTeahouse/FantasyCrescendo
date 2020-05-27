using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkMatch : DefaultMatch {

  const int resendDelay    = 500;     // in milliseconds
  const byte kLoadedHeader = 69;      // Nice

	readonly Lobby Lobby;
  readonly List<LobbyMember> _waitingMembers;
  readonly TaskCompletionSource<object> ready;

  BackrollMatchController controller;

	public NetworkMatch(Lobby lobby) {
		Lobby = Argument.NotNull(lobby);
    _waitingMembers = new List<LobbyMember>(Lobby.Members);
    _waitingMembers.Remove(lobby.Members.Me);

    controller = null;
    ready = new TaskCompletionSource<object>();

    Lobby.OnLobbyMessage += OnLobbyMessage;
    Lobby.OnDeleted += OnLobbyDelete;
    Lobby.OnMemberLeave += OnMemberLeave;
	}

	protected override IMatchController CreateMatchController(MatchConfig config ) {
    return (controller = new BackrollMatchController(Lobby));
	}

	protected override async Task InitializeMatch(MatchManager manager, MatchConfig config) {
    try {
      await base.InitializeMatch(manager, config);
      // Wait for all members of the lobby to be ready.
      do {
        BroadcastReady();
        await Task.WhenAny(Task.Delay(resendDelay), ready.Task);
      } while(ready.Task.IsCompleted);
      // Rethrow the error if an exception occured.
      await ready.Task;
      // Wait for the session to finish synchronizing
      await controller.StartSession();
    } finally {
      Unsubscribe();
    }
	}

  void Unsubscribe() {
    Lobby.OnNetworkMessage -= OnLobbyMessage;
    Lobby.OnDeleted -= OnLobbyDelete;
    Lobby.OnMemberLeave -= OnMemberLeave;
  }

  void OnLobbyDelete() => ready.SetException(new Exception("Network lobby was deleted."));
  void OnMemberLeave(LobbyMember member) {
    Debug.Log($"Remote Peer {member.Id.Id} left during initialization.");
    _waitingMembers.Remove(member);
  }

  unsafe void OnLobbyMessage(LobbyMember member, ReadOnlySpan<byte> msg) {
    // Stackalloc that must be bigger any ready message could ever be.
    var expected = stackalloc byte[256];
    var size = CreateReadyMessage(member, expected, 256);
    if (!BufferEquals(expected, size, msg)) {
      Debug.Log($"Unexpected message from {member.Id.Id}.");
      return;
    }
    _waitingMembers.Remove(member);
    Debug.Log($"Remote Peer {member.Id.Id} ready.");
    if (_waitingMembers.Count <= 0) {
      ready.SetResult(null);
      Debug.Log("All remote peers are ready!");
    }
  }

  unsafe void BroadcastReady() {
    Debug.Log("Broadcasting ready message.");
    var msg = stackalloc byte[256];
    var size = CreateReadyMessage(Lobby.Members.Me, msg, 256);
    Lobby.SendLobbyMessage(new ReadOnlySpan<byte>(msg, size));
  }

  unsafe int CreateReadyMessage(LobbyMember member, byte* buffer, int size) {
    var serializer = Serializer.Create(new Span<byte>(buffer, size));
    serializer.Write(kLoadedHeader);
    serializer.Write(member.Id.Id);
    return serializer.Position;
  }

  unsafe bool BufferEquals(byte* reference, int size, ReadOnlySpan<byte> buffer) {
    if (buffer.Length < size) return false;
    fixed (byte* ptr = buffer) {
        return UnsafeUtility.MemCmp(reference, ptr, size) == 0;
    }
  }

}
		
}
