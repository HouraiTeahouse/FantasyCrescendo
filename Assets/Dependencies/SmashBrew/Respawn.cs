using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public abstract class Respawn : MonoBehaviour {

        private BlastZone blastZone;

        void Awake() {
            blastZone = GetComponent<BlastZone>();
            if(!blastZone) {
                Destroy(this);
                return;
            }
            blastZone.OnBlastZoneExit += OnBlastZoneExit;
        }

        void OnDestroy() {
            if (blastZone)
                blastZone.OnBlastZoneExit -= OnBlastZoneExit;
        }

        protected virtual void OnBlastZoneExit(Character player) {
            Transform respawnPos = Stage.RespawnPosition;
            player.transform.position = respawnPos.position;
            player.transform.rotation = respawnPos.rotation;
            SmashGame.CreateRespawnPlatform(player);
        }

    }

}