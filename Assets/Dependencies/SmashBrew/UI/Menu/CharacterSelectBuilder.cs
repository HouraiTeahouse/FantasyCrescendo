using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// Constructs the player section of the in-match UI 
    /// </summary>
    public sealed class CharacterSelectBuilder : MonoBehaviour {
        [Header("Character Select")] [SerializeField] private RectTransform _characterContainer;

        [SerializeField] private RectTransform _character;

        [Header("Player Display")] [SerializeField] [Tooltip("The parent container object to add the created  displays to")] private RectTransform _playerContainer;

        [SerializeField] [Tooltip("Space prefab to buffer the UI on the sides")] private RectTransform _space;

        [SerializeField] [Tooltip("The Player Display Prefab to create.")] private RectTransform _playerDisplay;

        /// <summary>
        /// Unity Callback. Called on object instantation.
        /// </summary>
        void Awake() {
            CreateCharacterSelect();
            CreatePlayerDisplay();
        }

        /// <summary>
        /// Attaches a instantiated object to an existing parent.
        /// Also handles the request to rebuild the layout of the child and parent
        /// </summary>
        /// <param name="child">the instantiated child</param>
        /// <param name="parent">the parent to attache the child to</param>
        static void Attach(RectTransform child, Transform parent) {
            child.SetParent(parent, false);
            LayoutRebuilder.MarkLayoutForRebuild(child);
        }

        /// <summary>
        /// Construct the select area for characters.
        /// </summary>
        void CreateCharacterSelect() {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_characterContainer || !_character)
                return;

            foreach (CharacterData data in dataManager.Characters) {
                if (data == null || !data.IsVisible)
                    return;
                RectTransform character = Instantiate(_character);
                Attach(character, _characterContainer);
                character.name = data.name;
                character.GetComponentsInChildren<IDataComponent<CharacterData>>().SetData(data);
            }
        }

        /// <summary>
        /// Create the display for the character's selections and options
        /// </summary>
        void CreatePlayerDisplay() {
            if (!_playerContainer || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            foreach (Player player in Player.AllPlayers) {
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
    }
}