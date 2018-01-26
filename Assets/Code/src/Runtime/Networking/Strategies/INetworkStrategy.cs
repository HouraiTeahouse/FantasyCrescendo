using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public interface INetworkStrategy {

  ServerGameController CreateServer(INetworkServer server, MatchConfig config);
  ClientGameController CreateClient(INetworkClient server, MatchConfig config);

}

public static class INetworkStrategyExtensions {

  public static IMatchController CreateHost(this INetworkStrategy strategy,
																						NetworkHost host,
																						MatchConfig config) {
    var server = strategy.CreateServer(host.Server, config);
    var client = strategy.CreateClient(host.Client, config);
    return new NetworkHostController(client, server);
  }

}

}