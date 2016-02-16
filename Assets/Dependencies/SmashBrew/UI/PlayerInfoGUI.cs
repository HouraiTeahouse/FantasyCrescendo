using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Hourai.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : MonoBehaviour {

        [SerializeField]
        private GameObject _container;

        [SerializeField]
        private GameObject _displayPrefab;

        [SerializeField]
        private GameObject _spacePrefab;

        private GameObject _finalSpace;

        private GlobalMediator _mediator;

        void Awake() {
            _mediator = GlobalMediator.Instance;
            if (!_container) {
                enabled = false;
                return;
            }
            _mediator.Subscribe<PlayerSpawnEvent>(OnSpawnPlayer);
            if (!_spacePrefab)
                return;

            GameObject initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.transform.SetParent(_container.transform);
            _finalSpace.transform.SetParent(_container.transform);

            initialSpace.name = _spacePrefab.name;
            _finalSpace.name = _spacePrefab.name;
        }

        void OnDestroy() {
            _mediator.Unsubscribe<PlayerSpawnEvent>(OnSpawnPlayer);
        }

        private void OnSpawnPlayer(PlayerSpawnEvent eventArgs) {
            Player player = eventArgs.Player;
            if (!_displayPrefab || player == null)
                return;
            GameObject display = Instantiate(_displayPrefab);
            display.transform.SetParent(_container.transform, false);
            display.name = string.Format("Player Display {0}", player.PlayerNumber + 1);
            LayoutRebuilder.MarkLayoutForRebuild(display.transform as RectTransform);
            foreach (IPlayerGUIComponent component in display.GetComponentsInChildren<IPlayerGUIComponent>())
                component.SetPlayer(player);
            _finalSpace.transform.SetAsLastSibling();
        }

    }

}

