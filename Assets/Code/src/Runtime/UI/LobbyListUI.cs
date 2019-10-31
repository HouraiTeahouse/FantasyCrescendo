using HouraiTeahouse.Networking;
using HouraiTeahouse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class LobbyListUI : UIListBuilder<LobbyDisplay, Lobby>, IStateView<IEnumerable<Lobby>> {

  public void UpdateView(in IEnumerable<Lobby> state) =>
    BuildList(state);

  protected override void UpdateDisplay(LobbyDisplay lobbyDisplay, Lobby lobby) =>
    lobbyDisplay.UpdateView(lobby);

  public void Dispose() {
    ObjectUtility.Destroy(this);
    ObjectUtility.Destroy(Container);
  }

}

}