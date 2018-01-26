using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETConnection : INetworkConnection {

  public uint Id => (uint)ConnectionID;
  public MessageHandlers MessageHandlers { get; }
  public readonly UNETNetworkInterface NetworkInterface;
  public readonly int ConnectionID;
  readonly int HostID;

  internal UNETConnection(UNETNetworkInterface networkInterface, int connectionId) {
    NetworkInterface = networkInterface;
    HostID = NetworkInterface.HostID;
    MessageHandlers = networkInterface.MessageHandlers;
    ConnectionID = connectionId;
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