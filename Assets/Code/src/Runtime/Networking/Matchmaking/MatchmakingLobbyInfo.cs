using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public class LobbyInfo {

  public readonly IMatchmaker MatchMaker;

  public ulong Id;
  public string Name;
  public string OnwerName;
  public int CurrentPlayers;
  public int MaxPlayers;

  public uint StageID;

  internal LobbyInfo(IMatchmaker matchMaker) {
    MatchMaker = matchMaker;
  }

  public void Join() => MatchMaker.JoinLobby(this);

}

}