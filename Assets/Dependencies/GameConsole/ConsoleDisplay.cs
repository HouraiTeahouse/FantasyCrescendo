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

using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Console {
    /// <summary> Displays the most recent logs in the GameConsole on a Text object. REQUIRED COMPONENT: UnityEngine.UI.Text </summary>
    [RequireComponent(typeof(Text))]
    public class ConsoleDisplay : MonoBehaviour {
        // The Text component used to render the console history
        Text _displayText;

        // Used to build the log string from the console's history
        StringBuilder _textBuilder;

        // Has the Console been updated since the log was last rendered?
        bool _updated;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            _textBuilder = new StringBuilder();
            _updated = false;
            GameConsole.OnConsoleUpdate += Redraw;
            _displayText = GetComponent<Text>();
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() { GameConsole.OnConsoleUpdate -= Redraw; }

        /// <summary> Unity Callback. Called each time the Behaviour is enabled or the GameObject it is attached to is activated. </summary>
        void OnEnable() {
            if (_updated)
                Redraw();
            _updated = false;
        }

        /// <summary> Re-reads history and renders it to the Text component. </summary>
        void Redraw() {
            if (!isActiveAndEnabled) {
                _updated = true;
                return;
            }
            // Clears the current string
            _textBuilder.Length = 0;
            foreach (string log in GameConsole.History)
                _textBuilder.AppendLine(log);
            _displayText.text = _textBuilder.ToString();
        }
    }
}