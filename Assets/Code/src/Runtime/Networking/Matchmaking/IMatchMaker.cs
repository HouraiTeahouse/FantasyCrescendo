using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo.Matchmaking {

public interface IMatchmaker {

  Task<IEnumerable<LobbyInfo>> GetLobbies();
  Task<LobbyInfo> CreateLobby();
  Task JoinLobby(LobbyInfo lobby);

}

}
