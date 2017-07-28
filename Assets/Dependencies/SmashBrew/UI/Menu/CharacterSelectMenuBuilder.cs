using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> Constructs the player section of the in-match UI </summary>
    public sealed class CharacterSelectMenuBuilder : AbstractSelectMenuBuilder<CharacterData> {

        [Header("Player Display")]
        [SerializeField]
        [Tooltip("The parent container object to add the created  displays to")]
        RectTransform _playerContainer;

        [SerializeField]
        [Tooltip("The Player Display Prefab to create.")]
        RectTransform _playerDisplay;

        [SerializeField]
        [Tooltip("Space prefab to buffer the UI on the sides")]
        RectTransform _space;

        protected override void Awake() {
            base.Awake();
            CreatePlayerDisplay();
        }

        protected override IEnumerable<CharacterData> GetData() { return DataManager.Characters; }

        protected override void LogCreation(CharacterData data) {
            Log.Info("Creating Character Select Box for {0}", data.name);
        }

        /// <summary> Create the display for the character's selections and options </summary>
        void CreatePlayerDisplay() {
            if (!_playerContainer || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            var playerManager = PlayerManager.Instance;
            foreach (Player player in playerManager.LocalPlayers) {
                if (!player.Type.IsActive)
                    continue;
                RectTransform display = Instantiate(_playerDisplay);
                Attach(display, _playerContainer);

                display.name = "Player {0}".With(player.ID + 1);

                // Use the IDataComponent interfce to set the player data on all of the components that use it
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
