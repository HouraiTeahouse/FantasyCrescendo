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

using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A UI Text driver that displays the current damage of the a given player. </summary>
    public sealed class PlayerDamageDisplay : GradientNumberText,
                                              IDataComponent<Player> {
        Character _character;

        [SerializeField]
        [Tooltip("The font size of the suffix")]
        int suffixSize = 25;

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            if (data == null || data.PlayerObject == null)
                _character = null;
            else {
                _character = data.PlayerObject;
                Number = _character.Damage;
            }
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        protected override void Update() {
            base.Update();
            if (!Text || !_character)
                return;
            //TODO: Change this into a event
            bool visible = _character.isActiveAndEnabled;
            Text.enabled = visible;
            float value = Mathf.Floor(_character.Damage.CurrentDamage);
            if (visible && !Mathf.Approximately(Number, value))
                Number = value;
        }

        /// <summary>
        ///     <see cref="NumberText.ProcessNumber" />
        /// </summary>
        protected override string ProcessNumber(string number) {
            if (!_character)
                return number;
            return string.Format("{0}<size={1}>{2}</size>",
                number,
                suffixSize,
                _character.Damage.Type.Suffix);
        }
    }
}
