using HouraiTeahouse.Backroll;
using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public unsafe class BackrollMatchController : MatchController {

    // TODO(james7132): Extend this to allow for spectators
  static LobbyMember[] kNoSpectators = new LobbyMember[0];
  const int kFrameDelay = 3;

  readonly BackrollSessionConfig config;
  BackrollSession<PlayerInput> session;

  public BackrollMatchController(Lobby lobby) {
    var players = new List<LobbyMember>(lobby.Members);
    // Sorting by ID is consistent across all clients.
    players.Sort((a, b) => a.Id.Id.CompareTo(b.Id.Id));
    config = new BackrollSessionConfig  {
      Players = players.ToArray(),
      Spectators = kNoSpectators,
      Callbacks = new BackrollSessionCallbacks {
        SaveGameState = OnSaveGameState,
        LoadGameState = OnLoadGameState,
        FreeBuffer = OnFreeBuffer,
        AdvanceFrame = OnAdvanceFrame,
      },
    };
  }

  public Task StartSession() {
    var readyFuture = new TaskCompletionSource<object>();
    config.Callbacks.OnReady += () => {
      readyFuture.SetResult(null);
    };
    session = Backroll.Backroll.StartSession<PlayerInput>(config);
    return readyFuture.Task;
  }

  public override void Update() {
    if (CurrentState.StateID == MatchProgressionState.Intro) return;
    session.AdvanceFrame();
  }

  void OnSaveGameState(ref Sync.SavedFrame frame) {
    var outputSize = 256;
    while (true) {
      try {
        var temp = stackalloc byte[outputSize];
        var serializer = Serializer.Create(temp, (uint)outputSize);
        CurrentState.Serialize(ref serializer);
        var size = serializer.Position;
        var buffer = (byte*)UnsafeUtility.Malloc(size, 
                                                 UnsafeUtility.AlignOf<byte>(),
                                                 Allocator.Persistent);
        UnsafeUtility.MemCpy(buffer, temp, size);

        frame.Buffer = buffer;
        frame.Size = size;
        frame.Checksum = (int)Crc32.ComputeChecksum(buffer, size);
      } catch (IndexOutOfRangeException) {
        outputSize *= 2;
      }
    }
  }

  void OnLoadGameState(void* buffer, int len) {
    Assert.IsTrue(len >= 0);
    var deserializer = Deserializer.Create((byte*)buffer, (uint)len);
    CurrentState.Deserialize(ref deserializer);
  }

  void OnFreeBuffer(IntPtr buffer) => UnsafeUtility.Free((void*)buffer, Allocator.Persistent);

  void OnAdvanceFrame() {
    var input = new MatchInputContext { Previous = CurrentState.LastInput };
    session.SyncInput(&input.Current, UnsafeUtility.SizeOf<MatchInput>());
    var state = CurrentState;
    Simulation.Simulate(ref state, input);
    state.LastInput = input.Current;
    CurrentState = state;
  }


}

}