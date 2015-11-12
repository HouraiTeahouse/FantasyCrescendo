using System.Collections.Generic;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class Respawn : MonoBehaviour {

        private List<RespawnPlatform> respawnPoints;

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

        internal void AddRespawnPoint(RespawnPlatform point) {
            if(point)
                respawnPoints.Add(point);
        }

        protected virtual void OnBlastZoneExit(Character player) {
            RespawnPlatform respawnPoint = respawnPoints[player.Player.PlayerNumber];
            
            player.transform.position = respawnPoint.transform.position;
            player.transform.rotation = respawnPoint.transform.rotation;
        }

    }

}