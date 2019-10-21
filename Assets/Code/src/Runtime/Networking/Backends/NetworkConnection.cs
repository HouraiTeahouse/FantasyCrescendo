using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using HouraiTeahouse.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkConnection {

  readonly TaskCompletionSource<object> connectTask;

  public readonly int Id;
  public ConnectionStatus Status { get; private set; }
  public bool IsConnected => Status == ConnectionStatus.Connected;
  public ConnectionStats Stats => NetworkInterface.GetConnectionStats(Id);

  public MessageHandlers MessageHandlers { get; }
  public readonly INetworkInterface NetworkInterface;
  internal Task ConnectTask => connectTask.Task;

  internal NetworkConnection(INetworkInterface networkInterface, int connectionId) {
    NetworkInterface = networkInterface;
    MessageHandlers = networkInterface.MessageHandlers;
    Status = ConnectionStatus.Connecting;
    Id = connectionId;
    connectTask = new TaskCompletionSource<object>();
  }

  public void SendBytes(byte[] buffer, int size, NetworkReliablity reliability = NetworkReliablity.Reliable) {
    if (!IsConnected) return;
    NetworkInterface.Send(Id, buffer, size, reliability);
  }

  public void Disconnect()  {
    if (!IsConnected) return;
    NetworkInterface.Disconnect(Id);
  }

  internal bool ConnectInternal() {
    if (IsConnected) return false;
    Status = ConnectionStatus.Connected;
    connectTask.TrySetResult(null);
    return true;
  }

  internal void DisconnectInternal(Exception exception = null) {
    if (!IsConnected) return;
    Status = ConnectionStatus.Disconnected;
    if (exception == null) {
      connectTask.TrySetCanceled();
    } else {
      connectTask.TrySetException(exception);
    }
  }

}

}