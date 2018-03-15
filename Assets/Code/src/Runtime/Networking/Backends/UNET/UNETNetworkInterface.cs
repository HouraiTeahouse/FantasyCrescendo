using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public class UNETNetworkInterface : NetworkInterface {

  readonly IDictionary<NetworkReliablity, int> Channels;
  internal int HostID { get; set; }

  public UNETNetworkInterface() : base(NetworkMessage.MaxMessageSize) {
    Channels = new Dictionary<NetworkReliablity, int>();
  }

  public override void Initialize(uint port) {
    NetworkTransport.Init();

    var config = new ConnectionConfig();
    AddChannel(config, QosType.Reliable, NetworkReliablity.Reliable);
    AddChannel(config, QosType.Unreliable, NetworkReliablity.Unreliable);

    var hostTopology = new HostTopology(config, (int)GameMode.GlobalMaxPlayers);

    HostID = NetworkTransport.AddHost(hostTopology, (int)port);
  }

  public override async Task<NetworkConnection> Connect(string address, int port) {
    address = ResolveAddress(address);

    byte error;
    var connectionId = NetworkTransport.Connect(HostID, address, port, 0, out error);
    UNETUtility.HandleError(error);

    var connection = AddConnection(connectionId);
    await connection.ConnectTask.Task;
    return connection;
  }

  int GetChannelID(NetworkReliablity reliability) {
    int channelId;
    if (Channels.TryGetValue(reliability, out channelId)) {
      return channelId;
    }
    return Channels[NetworkReliablity.Reliable];
  }

  public override void Update() {
    if (HostID < 0) return;
    int connectionId, channelId, dataSize, bufferSize = NetworkMessage.MaxMessageSize;
    byte error;
    NetworkEventType evt;
    do {
      evt = NetworkTransport.ReceiveFromHost(HostID, out connectionId, out channelId, ReadBuffer, bufferSize, out dataSize, out error);
      switch (evt) {
        case NetworkEventType.Nothing: break;
        case NetworkEventType.ConnectEvent: OnNewConnection(connectionId); break;
        case NetworkEventType.DataEvent: OnRecieveData(connectionId, dataSize); break;
        case NetworkEventType.DisconnectEvent: OnDisconnect(connectionId, error); break;
        default:
          Debug.LogError($"Unkown network message type recieved: {evt}");
          break;
      }
    } while (evt != NetworkEventType.Nothing && !IsDisposed);
  }

  public override void Disconnect(int connectionId) {
    byte error;
    NetworkTransport.Disconnect(HostID, connectionId, out error);
    UNETUtility.HandleError(error);
  }

  public override void Dispose() {
    if (IsDisposed) return;
    base.Dispose();
    NetworkTransport.RemoveHost(HostID);
  }

  protected override void SendImpl(int connectionId, byte[] buffer, int count, NetworkReliablity reliablity) {
    byte error;
    var channelId = GetChannelID(reliablity);
    NetworkTransport.Send(HostID, connectionId, channelId, buffer, count, out error);
    UNETUtility.HandleError(error);
  }

  void AddChannel(ConnectionConfig config, QosType qos, NetworkReliablity reliablity) {
    Channels.Add(reliablity, config.AddChannel(qos));
  }

  public override ConnectionStats GetConnectionStats(int connectionId) {
    var stats = new ConnectionStats();
    byte error; 
    stats.TotalBytesOut = NetworkTransport.GetOutgoingFullBytesCountForConnection(HostID, connectionId, out error);
    UNETUtility.HandleError(error);
    stats.CurrentRTT = NetworkTransport.GetCurrentRTT(HostID, connectionId, out error);
    UNETUtility.HandleError(error);
    stats.IncomingPacketsCount = NetworkTransport.GetIncomingPacketCount(HostID, connectionId, out error);
    UNETUtility.HandleError(error);
    stats.IncomingPacketsLost = NetworkTransport.GetIncomingPacketLossCount(HostID, connectionId, out error);
    UNETUtility.HandleError(error);
    return stats;
  }

  string ResolveAddress(string address) {
    if (address.Equals("127.0.0.1") || address.Equals("localhost")) {
      return "127.0.0.1";
    }
    return address;
  }

}

}