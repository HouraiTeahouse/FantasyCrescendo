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
  }

  public override void Update() => NetworkClient.Update();

  public abstract void Dispose();

}

}