using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : MonoBehaviour {

        [SerializeField]
        private List<GameObject> _displays;

        private Queue<GameObject> _inactiveDisplays; 
        private GlobalMediator _mediator;

        void Awake() {
            _mediator = GlobalMediator.Instance;
            _mediator.Subscribe<PlayerSpawnEvent>(OnSpawnPlayer);
            _inactiveDisplays = new Queue<GameObject>(_displays.Where(go => go != null));
            foreach (GameObject go in _inactiveDisplays.Where(go => go.activeInHierarchy))
                go.SetActive(false);
        }

        void OnDestroy() {
            _mediator.Unsubscribe<PlayerSpawnEvent>(OnSpawnPlayer);
        }

        private void OnSpawnPlayer(PlayerSpawnEvent eventArgs) {
            Player player = eventArgs.Player;
            if (_inactiveDisplays.Count <= 0 || player == null)
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

