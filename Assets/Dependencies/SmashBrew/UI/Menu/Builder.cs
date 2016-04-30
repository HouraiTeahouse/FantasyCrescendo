using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    public abstract class Builder : MonoBehaviour {

        [Header("Player Display")] [SerializeField] [Tooltip("The parent container object to add the created  displays to")] private RectTransform _playerContainer;

        [SerializeField] [Tooltip("Space prefab to buffer the UI on the sides")] private RectTransform _space;

        [SerializeField] [Tooltip("The Player Display Prefab to create.")] private RectTransform _playerDisplay;

        void Awake() {
            CreateSelect ();
            CreatePlayerDisplay ();
        }

        public abstract void CreateSelect ();

        /// <summary>
        /// Create the display for the character's selections and options
        /// </summary>
        private void CreatePlayerDisplay() {
            if (!_playerContainer || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            foreach (Player player in Player.AllPlayers) {
                if (!player.IsActive)
                    continue;
                RectTransform display = Instantiate(_playerDisplay);
                Attach(display, _playerContainer);

                display.name = string.Format("Player {0}", player.PlayerNumber + 1);

                // Use the IDataComponent interface to set the player data on all of the components that use it
                display.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
            }

            if (!_space)
                return;

            //Create additional spaces to the left and right of the player displays to focus the attention on the center of the screen.
            RectTransform firstSpace = Instantiate(_space);
            RectTransform lastSpace = Instantiate(_space);
            Attach(firstSpace, _playerContainer);
            Attach(lastSpace, _playerContainer);
            firstSpace.SetAsFirstSibling();
            lastSpace.SetAsLastSibling();
        }

        public static void Attach(RectTransform child, Transform parent) {
            child.SetParent(parent, false);
            LayoutRebuilder.MarkLayoutForRebuild(child);
        }
    }
}