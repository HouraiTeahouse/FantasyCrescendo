using System.Collections.Generic;
using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class PlayerSpawnEvent {

        public Player Player; 
        public GameObject PlayerObject;

    }

    public class Spawn : MonoBehaviour {

        private Mediator _eventManager;

        [SerializeField]
        private Transform[] _spawnPoints;

        /// <summary>
        /// Unity Callback. Called on object instantation.
        /// </summary>
        void Awake() {
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<MatchStartEvent>(OnMatchStart);
        }

        /// <summary>
        /// Unity Callback. Called on object destruction.
        /// </summary>
        void OnDestroy() {
            _eventManager.Unsubscribe<MatchStartEvent>(OnMatchStart);
        }

        /// <summary>
        /// Spawns players when the match begins.
        /// </summary>
        /// <param name="startEventArgs"></param>
        void OnMatchStart(MatchStartEvent startEventArgs) {
            var i = 0;
            IEnumerator<Player> activePlayers = Player.ActivePlayers.GetEnumerator();
            while (i < _spawnPoints.Length && activePlayers.MoveNext()) {
                Player player = activePlayers.Current;
                Character runtimeCharacter = player.Spawn(_spawnPoints[i]);
                i++;
                if (runtimeCharacter == null)
                    continue;

                _eventManager.Publish(new PlayerSpawnEvent { Player = player, PlayerObject = runtimeCharacter.gameObject });

                //TODO: Fix this hack, get netplay working
                runtimeCharacter.gameObject.SetActive(true);
            }
        }

    }

}
