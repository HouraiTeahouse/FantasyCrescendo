using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    /// <summary>
    /// Constructs the player section of the 
    /// </summary>
    public class PlayerSelectDisplay : MonoBehaviour {

        [SerializeField]
        [Tooltip("The parent container object to add the created  displays to")]
        private RectTransform _container;

        [SerializeField]
        [Tooltip("Space prefab to buffer the UI on the sides")]
        private RectTransform _space;

        [SerializeField]
        [Tooltip("The Player Display Prefab to create.")]
        private RectTransform _playerDisplay;

        /// <summary>
        /// Unity Callback. Called before the object's first frame.
        /// </summary>
        void Start() {
            if (!_container || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            foreach (Player player in SmashGame.Players) {
                RectTransform display = Instantiate(_playerDisplay);

                // Child the new player display and order the UI system to rebuild the layout for it
                display.SetParent(_container, false);
                LayoutRebuilder.MarkLayoutForRebuild(display);

                display.name = string.Format("Player {0}", player.PlayerNumber);

                // Use the IPlayerGUIComponent interface to set the player data on all of the components that use it
                foreach (IPlayerGUIComponent guiComponent in display.GetComponentsInChildren<IPlayerGUIComponent>())
                    guiComponent.SetPlayerData(player);
            }

            if (!_space)
                return;

            //Create additional spaces to the left and right of the player displays to focus the attention on the center of the screen.
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