using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Networking;
using HouraiTeahouse.FantasyCrescendo.Networking.UNET;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

public abstract class NetworkInterfaceTestBase<T> where T : INetworkInterface, new() {

  NetworkHost Host;

  NetworkHostConfig CreateHostConfig() {
    return new NetworkHostConfig {
      ClientConfig = new NetworkClientConfig(),
      ServerConfig = new NetworkServerConfig {
        Port = 8888
      }
    };
  }

  [TearDown]
  public void TearDown() => Host.Dispose();

  PlayerInput RandomPlayerInput() {
    return new PlayerInput {
      Movement = Random.insideUnitCircle,
      Smash = Vector2.zero,
      Buttons = (byte)Random.Range(0, 255),
    };
  }

  IEnumerable<MatchInput> RandomInput(int count, int players) {
    for (int i = 0; i < count; i++ ) {
      yield return new MatchInput {
        PlayerInputs = Enumerable.Range(0, players).Select(_ => RandomPlayerInput()).ToArray()
      };
    }
  }

  IEnumerator RunTest(Action prepare, Func<bool> check, Action validate) {
    Host = NetworkHost.Create(typeof(T), CreateHostConfig());
    var connectTask = Host.Client.Connect("localhost", 8888);

    int count = 0;

    while (!connectTask.IsCompleted || Host.Server.Clients.Count <= 0) {
      Host.Update();
      yield return null;
      count++;
    }

    prepare();

    for (var i = 0; i < 1000; i++){
      Host.Update();
      if (check()) break;
      yield return null;
    }

    validate();
  }

	[UnityTest]
	public IEnumerator Server_StartMatch() {
    var server = new MatchConfig { 
      Time = 100,
      PlayerConfigs = new PlayerConfig[] {
        new PlayerConfig(),
        new PlayerConfig(),
        new PlayerConfig(),
        new PlayerConfig(),
      }
    };
    MatchConfig? client = null;

    return RunTest(() => {
      Host.Client.OnMatchStarted += config => client = config;
      foreach (var serverClient in Host.Server.Clients) {
        serverClient.StartMatch(server);
      }
    }, () => client != null, () => {
      Assert.AreEqual(server, client);
    });
	}

	[UnityTest]
	public IEnumerator Server_FinishMatch() {
    var server = new MatchResult();
    MatchResult? client = null;
    return RunTest(() => {
      Host.Client.OnMatchFinished += results => client = results;
      Host.Server.FinishMatch(server);
    }, () => client != null, () => {
      Assert.AreEqual(server, client);
    });
	}

	[UnityTest]
	public IEnumerator Server_BroadcastInput() {
    uint serverTimestamp = 42;
    var serverInputs = RandomInput(20, 4).ToArray();
    uint? clientTimestamp = null;
    IEnumerable<MatchInput> clientInputs = null;
    return RunTest(() => {
      Host.Client.OnRecievedInputs += (t, inputs) => {
        clientTimestamp = t;
        clientInputs = inputs;
      };
      Host.Server.BroadcastInput(serverTimestamp, serverInputs);
    }, () => clientTimestamp != null && clientInputs != null, () => {
      Assert.AreEqual(serverTimestamp, clientTimestamp);
      CollectionAssert.AreEqual(serverInputs, clientInputs);
    });
	}

	[UnityTest] 
	public IEnumerator Server_BroadcastState() {
    uint serverTimestamp = 300;
    var serverState = new MatchState();
    uint? clientTimestamp = null;
    MatchState clientState = null;
    return RunTest(() => {
      Host.Client.OnRecievedState += (t, state) => {
        clientTimestamp = t;
        clientState = state;
      };
      Host.Server.BroadcastState(serverTimestamp, serverState);
    }, () => clientState != null && clientTimestamp != null, () => {
      Assert.AreEqual(serverTimestamp, clientTimestamp);
      Assert.AreEqual(serverState, clientState);
    });
	}

	[UnityTest] public IEnumerator Client_SetReady_true()  => Test_IsReady(true);
	[UnityTest] public IEnumerator Client_SetReady_false()  => Test_IsReady(false);

  IEnumerator Test_IsReady(bool ready) {
    var client = ready;
    bool? server = null;
    return RunTest(() => {
      Host.Server.PlayerUpdated += player => server = player.IsReady;
      Host.Client.SetReady(client);
    }, () => server != null, () => {
      Assert.AreEqual(server, client);
    });
  }

	[UnityTest]
	public IEnumerator Client_SetConfig() {
    var client = new PlayerConfig();
    PlayerConfig? server = null;
    return RunTest(() => {
      Host.Server.PlayerUpdated += player => server = player.Config;
      Host.Client.SetConfig(client);
    }, () => server != null, () => {
      Assert.AreEqual(client, server);
    });
	}

	[UnityTest]
	public IEnumerator Client_SendInput() {
    uint clientTimestamp = 42;
    var clientInputs = RandomInput(20, 4).ToArray();
    uint? serverTimestamp = null;
    IEnumerable<MatchInput> serverInputs = null;
    return RunTest(() => {
      Host.Server.ReceivedInputs += (id, t, inputs) => {
        serverTimestamp = t;
        serverInputs = inputs;
      };
      Host.Client.SendInput(clientTimestamp, clientInputs);
    }, () => serverTimestamp != null && serverInputs == null, () => {
      Assert.AreEqual(clientTimestamp, serverTimestamp);
      CollectionAssert.AreEqual(clientInputs, serverInputs);
    });
	}

}

public class UNETNetworkInterfaceTest : NetworkInterfaceTestBase<UNETNetworkInterface> {}