using System;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class RespawnEvent {

        public bool Consumed;
        public Player Player;

    }

    [DisallowMultipleComponent]
    public class Respawn : MonoBehaviour {

        private List<Func<Player, bool>> _shouldRespawn;
        private Mediator _eventManager;
        
        public event Func<Player, bool> ShouldRespwan {
            add {
                if(_shouldRespawn == null)
                    _shouldRespawn = new List<Func<Player, bool>>();
                if(value != null)
                    _shouldRespawn.Add(value);
            }
            remove {
                if (_shouldRespawn == null)
                    _shouldRespawn = new List<Func<Player, bool>>();
                if (value != null)
                    _shouldRespawn.Remove(value);
            }
        }  

        void Awake() {
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<PlayerDieEvent>(PlayerDieEvent);
            if(_shouldRespawn == null)
                _shouldRespawn = new List<Func<Player, bool>>();
        }

        protected virtual void PlayerDieEvent(PlayerDieEvent eventArgs) {
            Player player = eventArgs.Player;
            if (_shouldRespawn.Count > 0 && _shouldRespawn.Any(check => !check(player))) {
                player.SpawnedCharacter.gameObject.SetActive(false);
                return;
            }
            var respawn = new RespawnEvent { Consumed = false, Player = player };
            _eventManager.Publish(respawn);
            if(!respawn.Consumed)
                throw new InvalidOperationException("Cannot respawn " + player.SpawnedCharacter.name + ".");
        }

    }

}