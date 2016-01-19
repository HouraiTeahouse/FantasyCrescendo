using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public class PlayerSelectDisplay : MonoBehaviour {

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _space;

        [SerializeField]
        private RectTransform _playerDisplay;

        void Start() {
            if (!_container || !_playerDisplay)
                return;
            foreach (Player player in SmashGame.Players) {
                RectTransform display = Instantiate(_playerDisplay);
                display.SetParent(_container, false);
                display.name = string.Format("Player {0}", player.PlayerNumber);
                LayoutRebuilder.MarkLayoutForRebuild(display);
                foreach(IPlayerGUIComponent guiComponent in display.GetComponentsInChildren<IPlayerGUIComponent>())
                    guiComponent.SetPlayerData(player);
            }
            if (_space) {
                RectTransform firstSpace = Instantiate(_space);
                RectTransform lastSpace = Instantiate(_space);
                firstSpace.SetParent(_container.transform, false);
                lastSpace.SetParent(_container.transform, false);
                firstSpace.SetAsFirstSibling();
                lastSpace.SetAsLastSibling();
                LayoutRebuilder.MarkLayoutForRebuild(firstSpace);
                LayoutRebuilder.MarkLayoutForRebuild(lastSpace);
            }
        }

    }

}