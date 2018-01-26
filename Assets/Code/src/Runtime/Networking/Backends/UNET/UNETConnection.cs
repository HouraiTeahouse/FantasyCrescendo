using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETConnection : INetworkConnection {

  public uint Id => (uint)ConnectionID;
  public MessageHandlers MessageHandlers { get; }
  public readonly UNETNetworkInterface NetworkInterface;
  public readonly int ConnectionID;
  internal readonly TaskCompletionSource<object> ConnectTask;
  readonly int HostID;

  internal UNETConnection(UNETNetworkInterface networkInterface, int connectionId) {
    NetworkInterface = networkInterface;
    HostID = NetworkInterface.HostID;
    MessageHandlers = networkInterface.MessageHandlers;
    ConnectionID = connectionId;
    ConnectTask = new TaskCompletionSource<object>();
  }

  public void SendBytes(byte[] buffer, int size, NetworkReliablity reliability = NetworkReliablity.Reliable) {
    byte error;
    var channelId = NetworkInterface.GetChannelID(reliability);
    NetworkTransport.Send(HostID, ConnectionID, channelId, buffer, size, out error);
    UNETUtility.HandleError(error);
  }

  public void Disconnect() {
    byte error;
    NetworkTransport.Disconnect(HostID, ConnectionID, out error);
    UNETUtility.HandleError(error);
  }

}

}