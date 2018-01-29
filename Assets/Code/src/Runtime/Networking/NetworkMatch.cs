using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkMatch : DefaultMatch {

	readonly NetworkManager NetworkManager;
  readonly TaskCompletionSource<object> ClientReady;
  readonly TaskCompletionSource<object> ServerReady;

	public NetworkMatch(NetworkManager networkManager) {
		NetworkManager = Argument.NotNull(networkManager);
    ClientReady = new TaskCompletionSource<object>();
    ServerReady = new TaskCompletionSource<object>();
	}

	protected override IMatchController CreateMatchController(MatchConfig config ) {
		return NetworkManager.CreateMatchController(config);
	}

	protected override async Task InitializeMatch(MatchManager manager, MatchConfig config) {
		if (NetworkManager.IsServer) {
      NetworkManager.Server.PlayerUpdated += ServerHandleClientReady;
      foreach(var client in NetworkManager.Server.Clients) {
        Debug.Log($"Setting {client.PlayerID} not ready");
        client.IsReady = false;
      }
		}
    if (NetworkManager.IsClient) {
      NetworkManager.Client.OnServerReady += ClientHandleServerReady;
    }
    await base.InitializeMatch(manager, config);
		// Wait for remote players to be ready
		var tasks = new List<Task>();
		if (NetworkManager.IsClient && !NetworkManager.Client.IsServerReady) {
      Debug.Log("Client: Set to Ready.");
      NetworkManager.Client.SetReady(true);
			tasks.Add(ClientReady.Task);
		}
		if (NetworkManager.IsServer && !AllClientsAreReady) {
      Debug.Log("Waiting on clients...");
			tasks.Add(ServerReady.Task);
		}
		await Task.WhenAll(tasks);
		if (NetworkManager.IsClient) {
      NetworkManager.Client.OnServerReady -= ClientHandleServerReady;
		}
		if (NetworkManager.IsServer) {
      NetworkManager.Server.PlayerUpdated -= ServerHandleClientReady;
		}
	}

	bool AllClientsAreReady => NetworkManager.Server.Clients.All(client => client.IsReady);

  void ClientHandleServerReady(bool isServerReady) {
    Debug.Log("Client: Server is Ready");
    if (isServerReady) {
      ClientReady.TrySetResult(true);
    }
  }

  void ServerHandleClientReady(NetworkClientPlayer player) {
    if (!player.IsReady) return;
    Debug.Log($"Server: Client {player.PlayerID} is Ready");
    if (!AllClientsAreReady) return;
    Debug.Log("Server: All Clients are Ready");
    NetworkManager.Server.SetReady(true);
    ServerReady.TrySetResult(true);
  }

}
		
}
