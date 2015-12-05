using System.Collections.Generic;
using UnityEngine;
using Hourai.Events;
using UnityEngine.Networking;

namespace Hourai.SmashBrew {

    public class SpawnPlayerEvent : IEvent {

        public Player Player; 
        public GameObject PlayerObject;

    }

    public class Spawn : MonoBehaviour {

        private Mediator _eventManager;
        private GameObject _player;

        [SerializeField]
        private Transform[] _spawnPoints;
        
        public bool Occupied { get { return _player != null; } } 

        void Awake() {
            _eventManager = GlobalEventManager.Instance;
            var i = 0;
            IEnumerator<Player> activePlayers = SmashGame.ActivePlayers.GetEnumerator();
            while (i < _spawnPoints.Length && activePlayers.MoveNext()) {
                Player player = activePlayers.Current;
                Character runtimeCharacter = player.Spawn(_spawnPoints[i]);
                i++;
                if (runtimeCharacter == null)
                    continue;

                _eventManager.Publish(new SpawnPlayerEvent { Player = player, PlayerObject = runtimeCharacter.gameObject });

                //TODO: Fix this hack, get netplay working
                runtimeCharacter.gameObject.SetActive(true);
            }
        }

    }

}