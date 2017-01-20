using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {
    
    [RequireComponent(typeof(DataManager))]
    [RequireComponent(typeof(PlayerManager))]
    public class SmashNetworkManager : NetworkManager {

        short localPlayerCount = 0;
        int playerCount = 0;

        DataManager DataManager { get; set; }
        PlayerManager PlayerManager { get; set; }

        void Awake() {
            DataManager = this.SafeGetComponent<DataManager>();
            PlayerManager = this.SafeGetComponent<PlayerManager>();
        }

        public class Messages {
            public const short UpdatePlayer = MsgType.Highest + 1;
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);
            // Update player when the server says so
            client.RegisterHandler(Messages.UpdatePlayer,
                msg => {
                    var update = msg.ReadMessage<UpdatePlayerMessage>();
                    var player = PlayerManager.GetMatchPlayer(update.ID);
                    update.UpdatePlayer(player);
                });
        }

        public override void OnClientConnect(NetworkConnection conn) {
            if (!clientLoadedScene) {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                ClientScene.Ready(conn);
                foreach (var player in PlayerManager.LocalPlayers.Where(p => p.Type.IsActive)) {
                    ClientScene.AddPlayer(conn, localPlayerCount, PlayerSelectionMessage.FromSelection(player.Selection));
                    localPlayerCount++;
                }
            }
        }

        public override void OnStartServer() {
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
                selection.Character = DataManager.Instance.Characters.Random();
                selection.Pallete = Mathf.FloorToInt(Random.value * selection.Character.PalleteCount);
            }
            var prefab = selection.Character.Prefab.Load();

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
            NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
            var player = PlayerManager.GetMatchPlayer(playerCount);
            var colorState = playerObj.GetComponentInChildren<ColorState>();
            if (colorState != null)
                colorState.Pallete = selection.Pallete;
            player.Selection = selection;
            player.Type = PlayerType.HumanPlayer;
            player.PlayerObject = playerObj;
            playerCount++;
            NetworkServer.SendToAll(Messages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
            base.OnServerRemovePlayer(conn, player);
            playerCount--;
        }

        public override void OnStopClient() {
            playerCount = 0;
            PlayerManager.ResetMatchPlayers();
        }

    }

}
