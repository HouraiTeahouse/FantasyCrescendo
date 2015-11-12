using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class Respawn : MonoBehaviour {

        private List<RespawnPlatform> respawnPoints;
        private BlastZone blastZone;
        private List<Func<Character, bool>> shouldRespawn;
        
        public event Func<Character, bool> ShouldRespwan {
            add {
                if(value != null)
                    shouldRespawn.Add(value);
            }
            remove {
                if (value != null)
                    shouldRespawn.Remove(value);
            }
        }  

        void Awake() {
            blastZone = GetComponent<BlastZone>();
            if(!blastZone) {
                Destroy(this);
                return;
            }
            blastZone.OnBlastZoneExit += OnBlastZoneExit;
            shouldRespawn = new List<Func<Character, bool>>();
            respawnPoints = new List<RespawnPlatform>(Resources.FindObjectsOfTypeAll<RespawnPlatform>());
            respawnPoints.Sort((t1, t2) => t1.name.CompareTo(t2.name));
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
            Debug.Log(player);
            if (shouldRespawn.Count > 0 && shouldRespawn.Any(check => !check(player))) {
                player.gameObject.SetActive(false);
                return;
            }
            foreach (var respawnPoint in respawnPoints) {
                if (respawnPoint && !respawnPoint.isActiveAndEnabled) {
                    respawnPoint.RespawnPlayer(player);
                    return;
                }
            }
            throw new InvalidOperationException("Cannot respawn another player! No available respawn platforms.");
        }

    }

}