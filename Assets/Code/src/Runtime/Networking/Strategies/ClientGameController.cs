using HouraiTeahouse.FantasyCrescendo.Matches;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {
		
public abstract class ClientGameController : MatchController, IDisposable {

  public INetworkClient NetworkClient { get; }

  protected ClientGameController(INetworkClient client, MatchConfig config) : base(config) {
    NetworkClient = Argument.NotNull(client);
    NetworkClient.OnDisconnect += Dispose;
  }

  public override void Update() => NetworkClient.Update();

  public override void Dispose() {
    NetworkClient.OnDisconnect -= Dispose;
    var matchManager = MatchManager.Instance;
    if (matchManager != null) {
      matchManager.EndMatch(new MatchResult {
        Resolution = MatchResolution.NoContest
      });
    }
    base.Dispose();
  }

}

}