using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    /// <summary>
    /// Constructs the player section of the 
    /// </summary>
    public class CharacterSelectBuilder : MonoBehaviour {

        [Header("Character Select")]
        [SerializeField]
        private RectTransform _characterContainer;

        [SerializeField]
        private RectTransform _character;

        [Header("Player Display")]
        [SerializeField]
        [Tooltip("The parent container object to add the created  displays to")]
        private RectTransform _playerContainer;

        [SerializeField]
        [Tooltip("Space prefab to buffer the UI on the sides")]
        private RectTransform _space;

        [SerializeField]
        [Tooltip("The Player Display Prefab to create.")]
        private RectTransform _playerDisplay;

        /// <summary>
        /// Unity Callback. Called before object's first frame.
        /// </summary>
        void Start() {
            CreateCharacterSelect();
            CreatePlayerDisplay();
        }

        static void Attach(RectTransform child, Transform parent) {
            child.SetParent(parent, false);
            LayoutRebuilder.MarkLayoutForRebuild(child);
        }

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
                foreach (ICharacterGUIComponent guiComponent in character.GetComponentsInChildren<ICharacterGUIComponent>())
                    guiComponent.SetCharacter(data);
            }

        } 

        void CreatePlayerDisplay() {
            if (!_playerContainer || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            foreach (Player player in SmashGame.Players) {
                RectTransform display = Instantiate(_playerDisplay);
                Attach(display, _playerContainer);

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
            Attach(firstSpace, _playerContainer);
            Attach(lastSpace, _playerContainer);
            firstSpace.SetAsFirstSibling();
            lastSpace.SetAsLastSibling();
        }

    }

}