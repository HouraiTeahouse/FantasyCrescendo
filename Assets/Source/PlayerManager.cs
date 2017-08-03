using HouraiTeahouse.SmashBrew.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerSet : IEnumerable<Player> {

        Player[] _players;

        public Player Get(int id) {
            if(!Check.Range(id, _players))
                throw new ArgumentOutOfRangeException();
            Assert.IsTrue(id < GameMode.Current.MaxPlayers);
            return _players[id];
        }

        public IEnumerator<Player> GetEnumerator() { return _players.Cast<Player>().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int Count {
            get { return _players.Length; }
        }

        public PlayerSet() {
            // Note: These objects are not intended to be destroyed and thus do not unresgister these event handlers
            // If there is a need to remove or replace them, this will need to be changed.
            GameMode.OnRegister += mode => RebuildPlayerArray();
            GameMode.OnChangeGameMode += mode => RebuildPlayerArray();
            _players = new Player[0];
            RebuildPlayerArray();
        }

        void RebuildPlayerArray() {
            var maxPlayers= !GameMode.All.Any() ? 0 : GameMode.All.Max(mode => mode.MaxPlayers);
            if (maxPlayers <= Count)
                return;
            var temp = new Player[maxPlayers];
            if(_players != null)
                Array.Copy(_players, temp , _players.Length);
            for (var i = 0; i < temp.Length; i++) {
                if(temp[i] == null)
                    temp[i] = new Player(i);
            }
            _players = temp;
        }

        public void ResetAll() {
            var blankSelection = new PlayerSelection();
            foreach (Player player in _players) {
                if (player == null)
                    continue;
                player.Selection = blankSelection;
                player.Type = PlayerType.None;
            }
        }

    }

    public class PlayerManager : MonoBehaviour {

        Player[] _localPlayers;
        Player[] _matchPlayers;

        public static PlayerManager Instance { get; private set; }

        public PlayerSet LocalPlayers { get; private set; }
        public PlayerSet MatchPlayers { get; private set; }

        Dictionary<PlayerConnection, Player> PlayerMap;

        public int MaxPlayers {
            get {
                Assert.IsTrue(LocalPlayers.Count == MatchPlayers.Count);
                return LocalPlayers.Count;
            }
        }

        short localPlayerCount;
        int playerCount = 0;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
            Config.Load();
            LocalPlayers = new PlayerSet();
            MatchPlayers = new PlayerSet();
            //TODO(james7132): Have this filled out externally.
            DataManager.LoadTask.Then(() => {
                var player = LocalPlayers.Get(0);
                player.Type = PlayerType.HumanPlayer;
                player.Selection = new PlayerSelection {
                    Character = DataManager.Characters.FirstOrDefault(c => c.IsSelectable)
                };
            });
            var networkManager = SmashNetworkManager.Instance;
            if (networkManager == null)
                return;
            networkManager.ClientStarted += client => {
                DestroyLeftoverPlayers();
                client.RegisterHandler(SmashNetworkMessages.UpdatePlayer,
                    msg => {
                        var update = msg.ReadMessage<UpdatePlayerMessage>();
                        var player = MatchPlayers.Get(update.ID);
                        update.UpdatePlayer(player);
                    });
                return Task.Resolved;
            };
            networkManager.ClientConnected += conn => {
                foreach (var player in LocalPlayers.Where(p => p.Type.IsActive)) {
                    ClientScene.AddPlayer(conn, localPlayerCount, PlayerSelectionMessage.FromSelection(player.Selection));
                    localPlayerCount++;
                }
            };
            networkManager.ClientDisconnected += conn => DestroyLeftoverPlayers();
            networkManager.ServerStarted += () => {
                DestroyLeftoverPlayers();
                // Update players when they change
                foreach (var player in MatchPlayers) {
                    player.Changed += () => {
                        if (Network.isServer) {
                            NetworkServer.SendToAll(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                        }
                    };
                }
            };
            networkManager.ServerReady += conn => {
                foreach (var player in MatchPlayers)
                    conn.Send(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
            };
            networkManager.ServerDisconnected += conn => {
                foreach (var playerController in conn.playerControllers)
                    RemovePlayer(conn, playerController);
            };
            networkManager.ServerAddedPlayer += AddPlayer;
            networkManager.ServerRemovedPlayer += RemovePlayer;
            networkManager.ClientStopped += () => {
                playerCount = 0;
                MatchPlayers.ResetAll();
            };
        }

        void AddPlayer(NetworkConnection conn, short playerControllerId, PlayerSelection selection) {
            if (playerControllerId < conn.playerControllers.Count && 
                conn.playerControllers[playerControllerId].IsValid && 
                conn.playerControllers[playerControllerId].gameObject != null) {
                Log.Error("There is already a player at that playerControllerId for this connections.");
                return;
            }

            var startPosition = SmashNetworkManager.Instance.GetStartPosition();
            var character = selection.Character;
            bool random = character == null;
            Analytics.CustomEvent("characterSelected", new Dictionary<string, object> {
                { "character", character != null ? character.name : "Random" },
                { "color" , selection.Pallete },
            });
            if (random) {
                Log.Info("No character was specfied, randomly selecting character and pallete...");
                selection.Character = DataManager.Characters.Random();
                selection.Pallete = Mathf.FloorToInt(Random.value * selection.Character.PalleteCount);
            }
            var sameCharacterSelections = new HashSet<PlayerSelection>(MatchPlayers.Select(p => p.Selection));
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
                var player = MatchPlayers.Get(playerCount);
                playerObj.name = "Player {0} ({1},{2})".With(playerCount + 1, selection.Character.name, selection.Pallete);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                var colorState = playerObj.GetComponentInChildren<ColorState>();
                if (colorState != null)
                    colorState.Pallete = selection.Pallete;
                player.Selection = selection;
                player.Type = PlayerType.HumanPlayer;
                player.PlayerObject = playerObj;
                playerCount++;
                NetworkServer.SendToAll(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                var playerConnection = new PlayerConnection {
                    ConnectionID = conn.connectionId,
                    PlayerControllerID = playerControllerId
                };
                PlayerMap[playerConnection] = player;
                playerObj.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                //player.Changed += () => playerObj.GetComponentInChildren<IDataComponent<Player>>().SetData(player);
            });
        }

        void RemovePlayer(NetworkConnection conn, PlayerController controller) {
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

        void DestroyLeftoverPlayers() {
            localPlayerCount = 0;
            playerCount = 0;
            var players = Resources.FindObjectsOfTypeAll<Character>();
            foreach (Character character in players) {
                if (character.gameObject.scene.isLoaded)
                    Destroy(character.gameObject);
            }
            MatchPlayers.ResetAll();
        }


    }

}
