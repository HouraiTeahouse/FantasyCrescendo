using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public sealed class LobbySetup : MonoBehaviour, IValidator<MatchConfig> {

  public GameSetupMenu GameSetupMenu;
  public CharacterSelectMenu CharacterSelectMenu;

  public NetworkGameSetup GameSetup;

  bool IValidator<MatchConfig>.IsValid(MatchConfig obj) {
    var networkManager = NetworkManager.Instance;
    if (networkManager == null || !networkManager.IsNetworkActive) return true;
    // TODO(james7132): Properly set this up again
    // if (networkManager.IsServer) {
    //   var baseConfig = ServerBuildBaseConfig();
    //   return GameSetupMenu.MainMenu.CurrentGameMode.IsValidConfig(baseConfig);
    // }
    // Non-host clients should not be able to start a match
    return false;
  }

}

}
