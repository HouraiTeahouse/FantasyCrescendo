using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using NetworkConnection = HouraiTeahouse.FantasyCrescendo.Networking.NetworkConnection;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETConnection : NetworkConnection {

  public override uint Id => (uint)ConnectionID;

  public override ConnectionStatus Status { get; protected internal set; }

  public override ConnectionStats Stats {
    get{
      var stats = new ConnectionStats();
      if (!IsConnected) return stats;
      byte error; 
      stats.TotalBytesOut = NetworkTransport.GetOutgoingFullBytesCountForConnection(HostID, ConnectionID, out error);
      UNETUtility.HandleError(error);
      stats.CurrentRTT = NetworkTransport.GetCurrentRTT(HostID, ConnectionID, out error);
      UNETUtility.HandleError(error);
      stats.IncomingPacketsCount = NetworkTransport.GetIncomingPacketCount(HostID, ConnectionID, out error);
      UNETUtility.HandleError(error);
      stats.IncomingPacketsLost = NetworkTransport.GetIncomingPacketLossCount(HostID, ConnectionID, out error);
      UNETUtility.HandleError(error);
      return stats;
    }
  }

  public override MessageHandlers MessageHandlers { get; }
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

  public override void SendBytes(byte[] buffer, int size, NetworkReliablity reliability = NetworkReliablity.Reliable) {
    if (!IsConnected) return;
    byte error;
    var channelId = NetworkInterface.GetChannelID(reliability);
    NetworkTransport.Send(HostID, ConnectionID, channelId, buffer, size, out error);
    UNETUtility.HandleError(error);
  }

  public override void Disconnect() {
    if (!IsConnected) return;
    byte error;
    NetworkTransport.Disconnect(HostID, ConnectionID, out error);
    UNETUtility.HandleError(error);
  }

}

}