using System.Linq;
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

        public override void OnClientConnect(NetworkConnection conn) {
            if (!clientLoadedScene) {
                // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
                ClientScene.Ready(conn);
                foreach (var player in PlayerManager.LocalPlayers.Where(p => p.Type.IsActive)) {
                    ClientScene.AddPlayer(conn, localPlayerCount, player.Selection.ToMessage());
                    localPlayerCount++;
                }
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            AddPlayer(conn, playerControllerId, new PlayerSelection());
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
            AddPlayer(conn, playerControllerId, PlayerSelection.FromMessage(extraMessageReader.ReadMessage<PlayerSelectionMessage>()));
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
                character = DataManager.Instance.Characters.Random();
            }
            var prefab = character.Prefab.Load();

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
            var player = PlayerManager.Instance.GetMatchPlayer(playerCount);
            player.Type = PlayerType.HumanPlayer;
            Mediator.Global.Publish(new PlayerSpawnEvent {Player = player, PlayerObject = playerObj});
            playerCount++;

        }

        public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
            base.OnServerRemovePlayer(conn, player);
            playerCount--;
        }

    }

}
