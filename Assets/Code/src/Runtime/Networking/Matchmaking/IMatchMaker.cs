using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public interface IMatchmaker {

  Type NetworkInterfaceType { get; }

  Task<IEnumerable<LobbyInfo>> GetLobbies();
  Task<LobbyInfo> CreateLobby();
  Task JoinLobby(LobbyInfo lobby);

}

}
