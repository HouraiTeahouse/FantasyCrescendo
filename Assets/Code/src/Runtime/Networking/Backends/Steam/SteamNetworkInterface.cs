using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking.Steam {

public sealed class SteamNetworkInterface : NetworkInterface {

  readonly IDictionary<int, CSteamID> connectionUsers;
  readonly IDictionary<CSteamID, int> connectionIds;

  Callback<P2PSessionRequest_t> callbackP2PSesssionRequest;
  Callback<P2PSessionConnectFail_t> callbackP2PConnectFail;
  Callback<LobbyChatUpdate_t> callbackLobbyChatUpdate;
  
  const int kMaxMessageSize = 1200;
  int lastConnectionId;
  CSteamID currentLobbyId;
  CSteamID lobbyOwner;

  public SteamNetworkInterface() : base(kMaxMessageSize) {
    ValidateSteamInitalized();
    lastConnectionId = 0;
    connectionUsers = new Dictionary<int, CSteamID>();
    connectionIds = new Dictionary<CSteamID, int>();
  }

  public override async Task Initialize(NetworkInterfaceConfiguration config) {
    ValidateSteamInitalized();
    await base.Initialize(config);
    Debug.Log($"[Steam] Steam User ID: {SteamUser.GetSteamID()}");
    callbackP2PSesssionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    callbackP2PConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
    callbackLobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

    if (config.Type != NetworkInterfaceType.Server) return;
    var type = config.ServerSteamLobbyType;
    var size = config.ServerSteamLobbyMaxSize;
    var result = await SteamMatchmaking.CreateLobby(type, size).ToTask<LobbyCreated_t>();
    
    if (SteamUtility.IsError(result.m_eResult)) {
      throw SteamUtility.CreateError(result.m_eResult);
    }
    var lobbyEnter = await SteamUtility.WaitFor<LobbyEnter_t>();
    currentLobbyId = new CSteamID(lobbyEnter.m_ulSteamIDLobby);
    Debug.Log($"[Steam] Created server lobby ID: {currentLobbyId}");
    SetLobbyData(currentLobbyId);
  }

  public override async Task<NetworkConnection> Connect(NetworkConnectionConfig config) {
    ValidateSteamInitalized();
    if (config.LobbyInfo == null) {
      throw new ArgumentException("Cannot connect to Steam networking except through a lobby.");
    }
    var id = new CSteamID(config.LobbyInfo.Id);
    Debug.Log($"[Steam] Joining lobby: {config.LobbyInfo.Id}");
    var lobbyEnter = await SteamMatchmaking.JoinLobby(id).ToTask<LobbyEnter_t>();
    var responseCode = (EChatRoomEnterResponse)lobbyEnter.m_EChatRoomEnterResponse;
    if (responseCode != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) {
      throw new NetworkingException($"Could not join lobby {lobbyEnter.m_ulSteamIDLobby}. Error: {responseCode}.");
    }
    await SteamUtility.WaitFor<LobbyDataUpdate_t>();
    currentLobbyId = new CSteamID(lobbyEnter.m_ulSteamIDLobby);
    lobbyOwner = SteamMatchmaking.GetLobbyOwner(currentLobbyId);
    // Send initial connection packet.
    Debug.Log($"[Steam] Sending initial connection packet to lobby owner {lobbyOwner}...");
    SendPacket(lobbyOwner, null, 0);
    var connection = AddConnection(lobbyOwner);
    var connectTask = connection.ConnectTask.Task;
    if (await Task.WhenAny(connectTask, Task.Delay(20000)) == connectTask) {
      Debug.Log("[Steam] Connected to host");
      return connection;
    } else {
      throw new NetworkingException("Could not connect to lobby host. Timeout: 20 seconds passed.");
    }
  }

  protected override void SendImpl(int connectionId, byte[] buffer, int count, 
                                   NetworkReliablity reliability) {
    ValidateSteamInitalized();
    EP2PSend ep2p = reliability == NetworkReliablity.Reliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliable;
    SendPacket(connectionUsers[connectionId], buffer, (uint)count, ep2p);
  }

  public override void Update() {
    ValidateSteamInitalized();
    uint dataSize;
    CSteamID userId;
    while (SteamNetworking.ReadP2PPacket(ReadBuffer, (uint)ReadBuffer.Length, out dataSize, out userId)) {
      int connectionId;
      if (!connectionIds.TryGetValue(userId, out connectionId)) continue;
      OnNewConnection(connectionId);
      OnRecieveData(connectionId, ReadBuffer, (int)dataSize);
    }
  }

  public override void Disconnect(int connectionId) {
    ValidateSteamInitalized();
    if (currentLobbyId != CSteamID.Nil) {
      CloseAllConnections();
    }
  }

  public override void Dispose() {
    CloseAllConnections();
    base.Dispose();
  }

  void SendPacket(CSteamID id, byte[] msg, uint size, EP2PSend reliability = EP2PSend.k_EP2PSendReliable) {
    if (!SteamNetworking.SendP2PPacket(id, msg, size, reliability)) {
      Debug.LogAssertion("Failed to send Steam packet!");
    }
  }
  
  // Steam Callbacks

  void OnP2PSessionRequest(P2PSessionRequest_t evt) {
    var id = evt.m_steamIDRemote;
    if (ExpectingClient(id)) {
      OnNewConnection(id);
      Debug.Log($"[Steam] New P2P Connection with player: {id}");
      // Send response packet to ack the connection creation
      Debug.Log($"[Steam] Sending connection response.");
      SendPacket(id, null, 0);
    } else {
      Debug.LogWarning("[Steam] Unexpected session request from " + id);
    }
  }

  void OnP2PSessionConnectFail(P2PSessionConnectFail_t evt)  {
    Debug.Log($"P2P Failed! {(EP2PSessionError)evt.m_eP2PSessionError}");
    OnDisconnect(evt.m_steamIDRemote);
  } 

  void OnLobbyChatUpdate(LobbyChatUpdate_t evt) {
    currentLobbyId = new CSteamID(evt.m_ulSteamIDLobby);
    var stateChange = (EChatMemberStateChange)evt.m_rgfChatMemberStateChange;
    if (stateChange == 0) {
      return;
    } else if ((stateChange | EChatMemberStateChange.k_EChatMemberStateChangeEntered) != 0) {
      Debug.Log($"[Steam] Lobby User Join: {evt.m_ulSteamIDUserChanged}");
    } else {
      Debug.Log($"[Steam] Lobby User Leave: {evt.m_ulSteamIDUserChanged}");
      OnDisconnect(new CSteamID(evt.m_ulSteamIDUserChanged));
    }
  }

  // Private Utility Functions

  void SetLobbyData(CSteamID lobbyId) {
    var success = SteamMatchmaking.SetLobbyData(lobbyId, "name", $"{SteamFriends.GetPersonaName()}'s Lobby");
    success |= SteamMatchmaking.SetLobbyData(lobbyId, "owner_name", SteamFriends.GetPersonaName());
    if (!success) {
      Debug.LogWarning("Error setting lobby info.");
    }
  }

  void OnDisconnect(CSteamID user) {
    if (user == SteamUser.GetSteamID()) {
      // User removed from lobby. 
    } else {
      if (user == lobbyOwner) {
        // Lobby owner has left, we need to leave the lobby to close it.
        CloseAllConnections();
      } else {
        RemoveConnection(user);
      }
    }
  }

  bool ExpectingClient(CSteamID id) {
    if (Config == null || currentLobbyId == CSteamID.Nil) return false;
    if (Config.Type == NetworkInterfaceType.Client) {
      // Only allow the lobby owner (the host/server) to connect to you if you are a client
      return lobbyOwner == id;
    } else {
      // Allow any user in the lobby to connect to the server
      var playerCount = SteamMatchmaking.GetNumLobbyMembers(currentLobbyId);
      for (var i = 0; i < playerCount; i++) {
        if (SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyId, i) == id) return true;
      }
      return false;
    }
  }

  NetworkConnection AddConnection(CSteamID user) {
    var connectionId = lastConnectionId;
    if (!connectionIds.TryGetValue(user, out connectionId)) {
      connectionId = lastConnectionId++;
    }
    connectionUsers[connectionId] = user;
    connectionIds[user] = connectionId;
    return AddConnection(connectionId);
  }

  NetworkConnection OnNewConnection(CSteamID user) {
    var connectionId = lastConnectionId;
    if (!connectionIds.TryGetValue(user, out connectionId)) {
      connectionId = lastConnectionId++;
    }
    connectionUsers[connectionId] = user;
    connectionIds[user] = connectionId;
    return OnNewConnection(connectionId);
  }

  void RemoveConnection(CSteamID user) {
    int id;
    if (!connectionIds.TryGetValue(user, out id)) return;
    SteamNetworking.CloseP2PSessionWithUser(user);
    connectionIds.Remove(user);
    connectionUsers.Remove(id);
  }

  void CloseAllConnections() {
    if (currentLobbyId == CSteamID.Nil) return;
    SteamMatchmaking.LeaveLobby(currentLobbyId);
    var playerCount = SteamMatchmaking.GetNumLobbyMembers(currentLobbyId);
    for (var i = 0; i < playerCount; i++) {
      var id = SteamMatchmaking.GetLobbyMemberByIndex(currentLobbyId, i);
      SteamNetworking.CloseP2PSessionWithUser(id);
    }
    Debug.Log($"[Steam] Left lobby {currentLobbyId}");
  }

  static void ValidateSteamInitalized() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot use Steam networking if the SteamManager is not initialzed.");
    }
  }

}

}