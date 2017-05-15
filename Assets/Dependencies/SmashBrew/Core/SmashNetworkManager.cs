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
    
    [RequireComponent(typeof(PlayerManager))]
    public class SmashNetworkManager : NetworkManager {

        short localPlayerCount = 0;
        int playerCount = 0;
        Dictionary<PlayerConnection, Player> PlayerMap;
        PlayerManager PlayerManager { get; set; }
        ITask ClientStarted;

        void Awake() {
            PlayerManager = this.SafeGetComponent<PlayerManager>();
            PlayerMap = new Dictionary<PlayerConnection, Player>();
        }

        public class Messages {
            public const short UpdatePlayer = MsgType.Highest + 1;
        }

        void DestroyLeftoverPlayers() {
            localPlayerCount = 0;
            playerCount = 0;
            var players = Resources.FindObjectsOfTypeAll<Character>();
            foreach (Character character in players) {
                if (character.gameObject.scene.isLoaded)
                    Destroy(character.gameObject);
            }
            PlayerManager.MatchPlayers.ResetAll();
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);
            DestroyLeftoverPlayers();
            ClientStarted = new Task();
            Log.Debug("Started client");
            // Update player when the server says so
            client.RegisterHandler(Messages.UpdatePlayer,
                msg => {
                    var update = msg.ReadMessage<UpdatePlayerMessage>();
                    var player = PlayerManager.MatchPlayers.Get(update.ID);
                    update.UpdatePlayer(player);
                });
            Task.All(DataManager.Characters.Select(character => character.Prefab.LoadAsync()
                .Then(prefab => {
                    if (prefab == null)
                        return;
                    Log.Debug(prefab);
#if UNITY_EDITOR
                    var instance = Instantiate(prefab) as GameObject;
                    ClientScene.RegisterPrefab(prefab, instance.GetComponent<NetworkIdentity>().assetId);
                    DestroyImmediate(instance);
#else
                    ClientScene.RegisterPrefab(prefab);
#endif
                })
            )).Then(() => {
                foreach (var pref in ClientScene.prefabs)
                    Log.Info("Registered Network Prefab: {0} ({1})", pref.Value.name, pref.Key);
                ClientStarted.Resolve();
                Log.Info("Client initialized.");
            });
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            DestroyLeftoverPlayers();
        }

        public override void OnClientConnect(NetworkConnection conn) {
            if (!clientLoadedScene) {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                ClientStarted.Then(() => {
                    Log.Debug("Client connecting");
                    ClientScene.Ready(conn);
                    foreach (var player in PlayerManager.LocalPlayers.Where(p => p.Type.IsActive)) {
                        ClientScene.AddPlayer(conn, localPlayerCount, PlayerSelectionMessage.FromSelection(player.Selection));
                        localPlayerCount++;
                    }
                });
            }
        }

        public override void OnStartServer() {
            DestroyLeftoverPlayers();
            // Update players when they change
            foreach (var player in PlayerManager.MatchPlayers) {
                player.Changed += () => {
                    if (Network.isServer) {
                        NetworkServer.SendToAll(Messages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                    }
                };
            }
        }

        public override void OnServerReady(NetworkConnection conn) {
            NetworkServer.SetClientReady(conn);
            foreach (var player in PlayerManager.MatchPlayers)
                conn.Send(Messages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            AddPlayer(conn, playerControllerId, new PlayerSelection());
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
            AddPlayer(conn, playerControllerId, extraMessageReader.ReadMessage<PlayerSelectionMessage>().ToSelection());
        }

        void AddPlayer(NetworkConnection conn, short playerControllerId, PlayerSelection selection) {
            if (playerControllerId < conn.playerControllers.Count && 
                conn.playerControllers[playerControllerId].IsValid && 
                conn.playerControllers[playerControllerId].gameObject != null) {
                Log.Error("There is already a player at that playerControllerId for this connections.");
                return;
            }

            var startPosition = GetStartPosition();
            var character = selection.Character;
            bool random = character == null;
            if (random) {
                Log.Info("No character was specfied, randomly selecting character and pallete...");
                selection.Character = DataManager.Characters.Random();
                selection.Pallete = Mathf.FloorToInt(Random.value * selection.Character.PalleteCount);
            }
            var sameCharacterSelections = new HashSet<PlayerSelection>(PlayerManager.MatchPlayers.Select(p => p.Selection));
            if (sameCharacterSelections.Contains(selection)) {
                bool success = false;
                for (var i = 0; i < selection.Character.PalleteCount; i++) {
                    selection.Pallete = i;
                    if (!sameCharacterSelections.Contains(selection)) {
                        success = true;
                        break;
                    }
                }
                if (!success) {
                    Log.Error("Two players made the same selection, and no remaining palletes remain. {0} doesn't have enough colors".With(selection.Character));
                    ClientScene.RemovePlayer(playerControllerId);
                    return;
                }
            }

            selection.Character.Prefab.LoadAsync().Then(prefab => {
                if (prefab == null) {
                    Log.Error("The character {0} does not have a prefab. Please add a prefab object to it.", selection.Character);
                    return;
                }

                if (prefab.GetComponent<NetworkIdentity>() == null) {
                    Log.Error(
                        "The character prefab for {0} does not have a NetworkIdentity. Please add a NetworkIdentity to it's prefab.",
                        selection.Character);
                    return;
                }

                GameObject playerObj;
                if (startPosition != null)
                    playerObj = Instantiate(prefab, startPosition.position, startPosition.rotation);
                else
                    playerObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                var player = PlayerManager.MatchPlayers.Get(playerCount);
                playerObj.name = "Player {0} ({1},{2})".With(playerCount + 1, selection.Character.name, selection.Pallete);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                var colorState = playerObj.GetComponentInChildren<ColorState>();
                if (colorState != null)
                    colorState.Pallete = selection.Pallete;
                player.Selection = selection;
                player.Type = PlayerType.HumanPlayer;
                player.PlayerObject = playerObj;
                playerCount++;
                NetworkServer.SendToAll(Messages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                var playerConnection = new PlayerConnection {
                    ConnectionID = conn.connectionId,
                    PlayerControllerID = playerControllerId
                };
                PlayerMap[playerConnection] = player;
            });
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController playerController) {
            base.OnServerRemovePlayer(conn, playerController);
            RemovePlayer(conn, playerController);
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            foreach (var playerController in conn.playerControllers)
                RemovePlayer(conn, playerController);
            base.OnServerDisconnect(conn);
        }

        void RemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController controller) {
            var playerConnection = new PlayerConnection {
                ConnectionID = conn.connectionId,
                PlayerControllerID = controller.playerControllerId
            };
            if(PlayerMap.ContainsKey(playerConnection)) {
                var player = PlayerMap[playerConnection];
                player.Type = PlayerType.None;
                player.Selection = new PlayerSelection();
                PlayerMap.Remove(playerConnection);
            }
            playerCount = Mathf.Max(0, playerCount - 1);
        }

        public override void OnStopClient() {
            playerCount = 0;
            PlayerManager.MatchPlayers.ResetAll();
        }

    }

}
