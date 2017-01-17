using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {
    
    [RequireComponent(typeof(DataManager))]
    [RequireComponent(typeof(PlayerManager))]
    public class SmashNetworkManager : NetworkManager {

        int playerCount = 0;

        DataManager DataManager { get; set; }
        PlayerManager PlayerManager { get; set; }

        void Awake() {
            DataManager = this.SafeGetComponent<DataManager>();
            PlayerManager = this.SafeGetComponent<PlayerManager>();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            if (playerControllerId < conn.playerControllers.Count && 
                conn.playerControllers[playerControllerId].IsValid && 
                conn.playerControllers[playerControllerId].gameObject != null) {
                Log.Error("There is already a player at that playerControllerId for this connections.");
                return;
            }

            var selection = PlayerManager.GetSelection(playerCount);
            var startPosition = GetStartPosition();
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

            GameObject player;
            if (startPosition != null)
                player = Instantiate(prefab, startPosition.position, startPosition.rotation);
            else
                player = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            Mediator.Global.Publish(new PlayerSpawnEvent {PlayerObject = player});
            playerCount++;
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
            base.OnServerRemovePlayer(conn, player);
            playerCount--;
        }

    }

}
