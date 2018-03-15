using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkConnection {

  public readonly int Id;
  public ConnectionStatus Status { get; protected internal set; }
  public bool IsConnected => Status == ConnectionStatus.Connected;
  public ConnectionStats Stats => NetworkInterface.GetConnectionStats(Id);

  public MessageHandlers MessageHandlers { get; }
  public readonly INetworkInterface NetworkInterface;
  internal readonly TaskCompletionSource<object> ConnectTask;

  internal NetworkConnection(INetworkInterface networkInterface, int connectionId) {
    NetworkInterface = networkInterface;
    MessageHandlers = networkInterface.MessageHandlers;
    Id = connectionId;
    ConnectTask = new TaskCompletionSource<object>();
  }

  public void SendBytes(byte[] buffer, int size, NetworkReliablity reliability = NetworkReliablity.Reliable) {
    if (!IsConnected) return;
    NetworkInterface.Send(Id, buffer, size, reliability);
  }

  public void Disconnect()  {
    if (!IsConnected) return;
    NetworkInterface.Disconnect(Id);
  }

}

}