using Steamworks;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public static class SteamLobbyManager {

  // static Callback<LobbyEnter_t> callbackLobbyEnter;
  // static Callback<LobbyDataUpdate_t> callbackLobbyDataUpdate;

  static ulong? currentLobbyId;

  public static bool InLobby => currentLobbyId != null;
  public static Action<ulong> OnJoinLobby;
  public static Action OnUpdateLobby;
  public static Action OnLeaveLobby;

  public static void Initialize() {
    // callbackLobbyEnter = Callback<LobbyEnter_t>.Create(evt => {
    //   currentLobbyId = evt.m_ulSteamIDLobby;
    //   OnJoinLobby?.Invoke(currentLobbyId.Value);
    // });
    // callbackLobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(evt => {
    // });
  }

  public static void CreateLobby(ELobbyType type = ELobbyType.k_ELobbyTypePublic,
                                       int maxLobbySize = (int)GameMode.GlobalMaxPlayers) {
    maxLobbySize = Math.Min(maxLobbySize, (int)GameMode.GlobalMaxPlayers);
    SteamMatchmaking.CreateLobby(type, maxLobbySize);
  }

  public static void LeaveLobby() {
    if (!InLobby) return;
    SteamMatchmaking.LeaveLobby(new CSteamID(currentLobbyId.Value));
  }

  public static async Task<CSteamID[]> GetLobbies() {
    var matchListPtr = await SteamMatchmaking.RequestLobbyList().ToTask<LobbyMatchList_t>();
    var matchList = new CSteamID[matchListPtr.m_nLobbiesMatching];
    for (var i = 0; i < matchList.Length; i++) {
      matchList[i] = SteamMatchmaking.GetLobbyByIndex(i);
    }
    return matchList;
  }

  public static Task<T> ToTask<T>(this SteamAPICall_t apiCall) {
    var completionSource = new TaskCompletionSource<T>();
    CallResult<T>.Create((callResult, failure) => {
      if (failure) {
        completionSource.TrySetException(new IOException("Steam IO Exception."));
      } else {
        completionSource.TrySetResult(callResult);
      }
    }).Set(apiCall);
    return completionSource.Task;
  }

}

}