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
  const byte kLaodedHeader = 69;      // Nice

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

    Lobby. OnLobbyMessage += OnLobbyMessage;
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

  void OnLobbyMessage(LobbyMember member, byte[] msg, uint size) {
    var expected = CreateReadyMessage(member);
    if (!BufferEquals(expected, msg, size)) {
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

  void BroadcastReady() {
    Debug.Log("Broadcasting ready message.");
    Lobby.SendLobbyMessage(CreateReadyMessage(Lobby.Members.Me));
  }

  unsafe byte[] CreateReadyMessage(LobbyMember member) {
    var buffer = stackalloc byte[256];
    var serializer = Serializer.Create(buffer, 256);
    serializer.Write(kLaodedHeader);
    serializer.Write(member.Id.Id);
    return serializer.ToArray();
  }

  unsafe bool BufferEquals(byte[] a, byte[] b, uint size) {
    if (size < a.Length) return false;
    fixed (byte* aPtr = a, bPtr = b) {
      return UnsafeUtility.MemCmp(aPtr, bPtr, a.Length) == 0;
    }
  }

}
		
}
