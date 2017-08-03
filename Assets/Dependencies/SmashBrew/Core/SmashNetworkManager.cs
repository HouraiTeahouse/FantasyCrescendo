using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public struct PlayerConnection {
        public int ConnectionID;
        public int PlayerControllerID;
        public override int GetHashCode() { return ConnectionID * 37 + PlayerControllerID; }
    }

    public class SmashNetworkMessages {
        public const short UpdatePlayer = MsgType.Highest + 1;
    }
    
    [RequireComponent(typeof(PlayerManager))]
    public class SmashNetworkManager : NetworkManager {

        public static SmashNetworkManager Instance { get; private set; }

        ITask _clientStartedTask;
        List<Func<NetworkClient, ITask>> _clientStartedCallbacks;

        public event Func<NetworkClient, ITask> ClientStarted {
            add { _clientStartedCallbacks.Add(value); }
            remove { _clientStartedCallbacks.Remove(value); }
        }

        public event Action<NetworkConnection> ClientConnected;
        public event Action<NetworkConnection> ClientDisconnected;
        public event Action ClientStopped;

        public event Action<NetworkConnection, short, PlayerSelection> ServerAddedPlayer;
        public event Action<NetworkConnection, PlayerController> ServerRemovedPlayer;
        public event Action ServerStarted;
        public event Action<NetworkConnection> ServerReady;
        public event Action<NetworkConnection> ServerDisconnected;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
            _clientStartedCallbacks = new List<Func<NetworkClient, ITask>>();
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);
            _clientStartedTask = new Task();
            Log.Info("Starting client initialization.");
            _clientStartedTask = Task.All(_clientStartedCallbacks.Select(fun => fun(client)))
                .Then(() => {
                    _clientStartedTask.Resolve();
                    Log.Info("Client initialized.");
                });
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            ClientDisconnected.SafeInvoke(conn);
        }

        public override void OnClientConnect(NetworkConnection conn) {
            if (!clientLoadedScene) {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                _clientStartedTask.Then(() => {
                    Log.Debug("Client connecting...");
                    ClientScene.Ready(conn);
                    ClientConnected.SafeInvoke(conn);
                });
            }
        }

        public override void OnStartServer() {
            ServerStarted.SafeInvoke();
        }

        public override void OnServerReady(NetworkConnection conn) {
            NetworkServer.SetClientReady(conn);
            ServerReady.SafeInvoke(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            ServerAddedPlayer.SafeInvoke(conn, playerControllerId, new PlayerSelection());
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
            ServerAddedPlayer.SafeInvoke(conn, playerControllerId, extraMessageReader.ReadMessage<PlayerSelectionMessage>().ToSelection());
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
            base.OnServerRemovePlayer(conn, playerController);
            ServerRemovedPlayer.SafeInvoke(conn, playerController);
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            ServerDisconnected.SafeInvoke(conn);
            base.OnServerDisconnect(conn);
        }

        public override void OnStopClient() {
            base.OnStopClient();
            ClientStopped.SafeInvoke();
        }

    }

}
