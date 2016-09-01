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

using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A Component that displays a Character (or a Player's Character) name on a UI Text object </summary>
    public sealed class NameText : AbstractLocalizedText, IDataComponent<Player>,
                                   IDataComponent<CharacterData> {
        [SerializeField]
        [Tooltip("Capitalize the character's name?")]
        bool _capitalize;

        [SerializeField]
        [Tooltip("The character who's name is to be displayed")]
        CharacterData _character;

        Player _player;

        [SerializeField]
        [Tooltip("Use the character's short or long name?")]
        bool shortName;

        /// <summary>
        ///     <see cref="IDataComponent{CharacterData}.SetData" />
        /// </summary>
        public void SetData(CharacterData data) {
            if (data == null)
                Text.text = string.Empty;
            else
                LocalizationKey = shortName ? data.ShortName : data.FullName;
        }

        /// <summary>
        ///     <see cref="IDataComponent{Player}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            if (_player != null)
                _player.Changed -= PlayerChange;
            _player = data;
            if (_player != null)
                _player.Changed += PlayerChange;
            PlayerChange();
        }

        protected override void Awake() {
            base.Awake();
            SetData(_character);
        }

        /// <summary>
        ///     <see cref="AbstractLocalizedText.Process" />
        /// </summary>
        protected override string Process(string val) {
            return !_capitalize ? val : val.ToUpperInvariant();
        }

        /// <summary> Events callback. Called whenever the Player changes. </summary>
        void PlayerChange() {
            SetData(_player == null ? null : _player.Selection.Character);
        }
    }
}
