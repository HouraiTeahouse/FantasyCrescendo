using System;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class RespawnEvent : IEvent {

        public bool consumed;
        public Character player;

    }

    [DisallowMultipleComponent]
    public class Respawn : MonoBehaviour {

        private List<Func<Character, bool>> shouldRespawn;
        private Mediator eventManager;
        
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
            eventManager = GlobalEventManager.Instance;
            eventManager.Subscribe<BlastZoneExit>(OnBlastZoneExit);
            shouldRespawn = new List<Func<Character, bool>>();
        }

        protected virtual void OnBlastZoneExit(BlastZoneExit eventArgs) {
            Character player = eventArgs.player;
            if (shouldRespawn.Count > 0 && shouldRespawn.Any(check => !check(player))) {
                player.gameObject.SetActive(false);
                return;
            }
            var respawn = new RespawnEvent { consumed = false, player = player };
            eventManager.Publish(respawn);
            if(!respawn.consumed)
                throw new InvalidOperationException("Cannot respawn " + player.name + ".");
        }

    }

}