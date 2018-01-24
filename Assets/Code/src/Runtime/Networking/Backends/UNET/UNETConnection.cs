using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETConnection : INetworkConnection {

  public uint Id => (uint)ConnectionID;
  public MessageHandlers MessageHandlers { get; }
  public readonly UNETNetworkInterface NetworkInterface;
  public readonly int ConnectionID;

  internal UNETConnection(UNETNetworkInterface networkInterface, int connectionId) {
    MessageHandlers = networkInterface.MessageHandlers;
    NetworkInterface = networkInterface;
    ConnectionID = connectionId;
  }

  public void SendBytes(byte[] buffer, int size, NetworkReliablity reliability = NetworkReliablity.Reliable) {
    byte error;
    var socketId = NetworkInterface.HostID;
    var channelId = NetworkInterface.GetChannelID(reliability);
    NetworkTransport.Send(socketId, ConnectionID, channelId, buffer, size, out error);
    UNETUtility.HandleError(error);
  }

  public void Disconnect() {

  }

}

}