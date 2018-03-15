using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking.UNET {

public sealed class SteamNetworkInterface : NetworkInterface {

  readonly IDictionary<int, CSteamID> connectionUsers;
  readonly IDictionary<CSteamID, int> connectionIds;

  Callback<P2PSessionRequest_t> callbackP2PSesssionRequest;
  Callback<P2PSessionConnectFail_t> callbackP2PConnectFail;
  
  const int kMaxMessageSize = 1200;
  int lastConnectionId;
  CSteamID? currentLobbyId;

  public SteamNetworkInterface() : base(kMaxMessageSize) {
    ValidateSteamInitalized();
    lastConnectionId = 0;
    connectionUsers = new Dictionary<int, CSteamID>();
    connectionIds = new Dictionary<CSteamID, int>();
  }

  public override void Initialize(uint port) {
    ValidateSteamInitalized();
    callbackP2PSesssionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    callbackP2PConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
  }

  void OnP2PSessionRequest(P2PSessionRequest_t evt) {
    var id = evt.m_steamIDRemote;
    if (ExpectingClient(id)) {
      OnNewConnection(lastConnectionId++);
      connectionUsers[lastConnectionId] = id;
      connectionIds[id] = lastConnectionId;
    } else {
      Debug.LogWarning("Unexpected session request from " + id);
    }
  }

  void OnP2PSessionConnectFail(P2PSessionConnectFail_t evt) {
  }

  bool ExpectingClient(CSteamID id) {
    if (currentLobbyId == null) return false;
    // Check if player is in the lobby 
    var playerCount = SteamMatchmaking.GetNumLobbyMembers(currentLobbyId.Value);
    for (var i = 0; i < playerCount; i++) {
      if (SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyId.Value, i) == id) return true;
    }
    return false;
  }

  public override Task<NetworkConnection> Connect(string address, int port) {
    throw new NotImplementedException();
  }

  protected override void SendImpl(int connectionId, byte[] buffer, int count, 
                                NetworkReliablity reliability) {
    EP2PSend ep2p = reliability == NetworkReliablity.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliableNoDelay;
    SteamNetworking.SendP2PPacket(connectionUsers[connectionId], buffer, (uint)count, ep2p);
  }

  public override void Update() {
    uint dataSize;
    CSteamID userId;
    while (SteamNetworking.ReadP2PPacket(ReadBuffer, (uint)ReadBuffer.Length, out dataSize, out userId)) {
      OnRecieveData(connectionIds[userId], (int)dataSize);
    }
  }

  public override void Disconnect(int connectionId) {
    SteamNetworking.CloseP2PSessionWithUser(new CSteamID());
  }

  static void ValidateSteamInitalized() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot use Steam networking if the SteamManager is not initialzed.");
    }
  }

}

}