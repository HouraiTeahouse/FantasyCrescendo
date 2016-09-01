// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> Constructs the player section of the in-match UI </summary>
    public sealed class CharacterSelectMenuBuilder :
        AbstractSelectMenuBuilder<CharacterData> {
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

        protected override IEnumerable<CharacterData> GetData() {
            return DataManager.Instance.Characters;
        }

        protected override void LogCreation(CharacterData data) {
            Log.Info("Creating Character Select Box for {0}", data.name);
        }

        /// <summary> Create the display for the character's selections and options </summary>
        void CreatePlayerDisplay() {
            if (!_playerContainer || !_playerDisplay)
                return;

            //Create a player display for as many players as the game can support
            foreach (Player player in Player.AllPlayers) {
                if (!player.IsActive)
                    continue;
                RectTransform display = Instantiate(_playerDisplay);
                Attach(display, _playerContainer);

                display.name = "Player {0}".With(player.ID + 1);

                // Use the IDataComponent interfce to set the player data on all of the components that use it
                display.GetComponentsInChildren<IDataComponent<Player>>()
                    .SetData(player);
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
