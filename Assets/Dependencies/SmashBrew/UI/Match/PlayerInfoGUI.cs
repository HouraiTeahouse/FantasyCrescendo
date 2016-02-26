using UnityEngine;
using HouraiTeahouse.Events;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerInfoGUI : PrefabFactoryEventHandler<RectTransform, PlayerSpawnEvent> {

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _spacePrefab;

        private RectTransform _finalSpace;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_container) {
                enabled = false;
                return;
            }
            if (!_spacePrefab)
                return;

            RectTransform initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.SetParent(_container.transform);
            _finalSpace.SetParent(_container.transform);

            initialSpace.name = _spacePrefab.name;
            _finalSpace.name = _spacePrefab.name;
        }

        protected override bool ShouldCreate(PlayerSpawnEvent eventArgs) {
            return base.ShouldCreate(eventArgs) && eventArgs.Player != null;
        }

        protected override RectTransform Create(PlayerSpawnEvent eventArgs) {
            Player player = eventArgs.Player;
            RectTransform display = base.Create(eventArgs); 
            display.transform.SetParent(_container.transform, false);
            display.name = string.Format("Player Display {0}", player.PlayerNumber + 1);
            LayoutRebuilder.MarkLayoutForRebuild(display);
            foreach (IPlayerGUIComponent component in display.GetComponentsInChildren<IPlayerGUIComponent>())
                component.SetPlayer(player);
            _finalSpace.transform.SetAsLastSibling();
            return display;
        }

    }

}

