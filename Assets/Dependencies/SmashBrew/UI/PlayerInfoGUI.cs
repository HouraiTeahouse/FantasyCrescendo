using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : MonoBehaviour {

        [SerializeField]
        private PlayerIndicator playerIndicatorPrefab;

        [SerializeField]
        private List<GameObject> _displays;

        private Queue<GameObject> _inactiveDisplays; 
        private GlobalEventManager _eventManager;

        void Awake() {
            _eventManager = GlobalEventManager.Instance;
            _eventManager.Subscribe<SpawnPlayerEvent>(OnSpawnPlayer);
            _inactiveDisplays = new Queue<GameObject>(_displays.Where(go => go != null));
            foreach (GameObject go in _inactiveDisplays.Where(go => go.activeInHierarchy))
                go.SetActive(false);
        }

        void OnDestroy() {
            _eventManager.Unsubscribe<SpawnPlayerEvent>(OnSpawnPlayer);
        }

        private void OnSpawnPlayer(SpawnPlayerEvent eventArgs) {
            Player player = eventArgs.Player;
            if (_inactiveDisplays.Count < 0 || player == null)
                return;
            GameObject display = _inactiveDisplays.Dequeue();
            display.SetActive(true);
            display.name = "Player " + (player.PlayerNumber + 1) + " Display";

            // Update the display with the Character information
            foreach (IPlayerGUIComponent component in display.GetComponentsInChildren<IPlayerGUIComponent>())
                component.SetPlayerData(eventArgs.Player);
        }

    }

}

