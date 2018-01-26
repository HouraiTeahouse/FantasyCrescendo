using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public abstract class ServerGameController : GameController, IDisposable {

  public INetworkServer NetworkServer { get; }

  protected ServerGameController(INetworkServer server, MatchConfig config) : base(config) {
    NetworkServer = Argument.NotNull(server);
  }

  public override void Update() => NetworkServer.Update();

  public abstract void Dispose();

}

}