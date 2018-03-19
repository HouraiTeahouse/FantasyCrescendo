using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class SteamMatchmaker : IMatchmaker {

  readonly List<LobbyInfo> lobbies;

  const string kNameKey = "name";
  const string kOwnerName = "owner_name";

  public SteamMatchmaker() {
    lobbies = new List<LobbyInfo>();
  }

  public async Task<IEnumerable<LobbyInfo>> GetLobbies() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot fetch lobbies if SteamManager is not initalized.");
    }
    var lobbyList = await SteamMatchmaking.RequestLobbyList().ToTask<LobbyMatchList_t>();
    lobbies.Clear();
    for (var i = 0; i < lobbyList.m_nLobbiesMatching; i++) {
      var id = SteamMatchmaking.GetLobbyByIndex(i);
      lobbies.Add(CreateLobbyInfo(id));
    }
    return lobbies;
  }

  public void Update() {
    foreach (var lobby in lobbies) {
      UpdateLobbyInfo(lobby);
    }
  }

  public async Task<LobbyInfo> CreateLobby() {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot create Steam lobby if SteamManager is not initalized.");
    }
    var apiCall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 
                                               (int)GameMode.GlobalMaxPlayers);
    var createdLobby = await apiCall.ToTask<LobbyCreated_t>();
    if (createdLobby.m_eResult == EResult.k_EResultOK) {
      var lobbyInfo = CreateLobbyInfo(new CSteamID(createdLobby.m_ulSteamIDLobby));
      lobbies.Add(lobbyInfo);
      return lobbyInfo;
    } else {
      throw new Exception($"Failed to create lobby: {createdLobby.m_eResult}");
    }
  }

  LobbyInfo CreateLobbyInfo(CSteamID id) {
    var info = new LobbyInfo(this);
    info.Id = id.m_SteamID;
    UpdateLobbyInfo(info);
    return info;
  }

  void UpdateLobbyInfo(LobbyInfo info) {
    var lobbyId = new CSteamID(info.Id);
    info.Name = SteamMatchmaking.GetLobbyData(lobbyId, kNameKey);
    info.OnwerName = SteamMatchmaking.GetLobbyData(lobbyId, kOwnerName);
    info.CurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyId);
    info.MaxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobbyId);
  }

  public async Task JoinLobby(LobbyInfo lobby) {
    if (!SteamManager.Initialized) {
      throw new InvalidOperationException("Cannot join Steam lobby if SteamManager is not initalized.");
    }
    var lobbyEnter = await SteamMatchmaking.JoinLobby(new CSteamID(lobby.Id)).ToTask<LobbyEnter_t>();
    var response = (EChatRoomEnterResponse)lobbyEnter.m_EChatRoomEnterResponse;
    if (response != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess) {
      throw new Exception($"Failed to join lobby: {response}");
    }
  }

}

}