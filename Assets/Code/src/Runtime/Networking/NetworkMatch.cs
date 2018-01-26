using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.FantasyCrescendo.Networking {

public class NetworkMatch : DefaultMatch {

	readonly NetworkManager NetworkManager;

	public NetworkMatch(NetworkManager networkManager) {
		NetworkManager = Argument.NotNull(networkManager);
	}

	protected override IMatchController CreateMatchController(MatchConfig config ) {
		return NetworkManager.CreateMatchController(config);
	}

	protected override async Task InitializeMatch(MatchManager manager, MatchConfig config) {
		await base.InitializeMatch(manager, config);
		// Wait for remote players to be ready
		var tasks = new List<Task>();
		if (NetworkManager.IsClient) {
			tasks.Add(WaitForServerToBeReady());
		}
		if (NetworkManager.IsServer) {
			tasks.Add(WaitForClientsToBeReady());
		}
		await Task.WhenAll(tasks);
	}

	bool AllClientsAreReady => NetworkManager.Server.Clients.All(client => client.IsReady);

	async Task WaitForServerToBeReady() {
		Assert.IsNotNull(NetworkManager.Client);
		NetworkManager.Client.SetReady(true);
		var taskCompletion = new TaskCompletionSource<object>();
		Action<bool> handler = isReady => {
			if (isReady) {
				taskCompletion.TrySetResult(new object());
			}
		};
		NetworkManager.Client.OnServerReady += handler;
		await taskCompletion.Task;
		NetworkManager.Client.OnServerReady -= handler;
	}

	async Task WaitForClientsToBeReady() {
		Assert.IsNotNull(NetworkManager.Server);
		if (!AllClientsAreReady) {
			var taskCompletion = new TaskCompletionSource<object>();
			Action<NetworkClientPlayer> handler = player => {
				if (AllClientsAreReady) {
					taskCompletion.TrySetResult(new object());
				}
			};
			NetworkManager.Server.PlayerUpdated += handler;
			await taskCompletion.Task;
			Assert.IsTrue(AllClientsAreReady);
			NetworkManager.Server.PlayerUpdated -= handler;
		}
		NetworkManager.Server.SetReady(true);
	}

}
		
}
