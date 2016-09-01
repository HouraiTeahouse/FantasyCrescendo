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
using UnityEngine.UI;

namespace HouraiTeahouse.Console {

    /// <summary> Turns a Text UI object into a entry line for GameConsole. Automatically reads, displays, and enters keyboard
    /// entered commands into the console. REQUIRED COMPONENT: UnityEngine.UI.Text </summary>
    [RequireComponent(typeof(Text))]
    public class ConsoleCommandEntry : MonoBehaviour {
        Text _text;

        /// <summary> Unity Callback. Called on instantiation. </summary>
        void Awake() { _text = GetComponent<Text>(); }

        /// <summary> Unity Callback. Called once per frame. </summary>
        void Update() {
            // Do nothing if not enabled.
            if (!isActiveAndEnabled)
                return;

            // Enter and execute command if Return/Enter is pressed
            string input = Input.inputString;
            if (input.Contains("\n") || input.Contains("\r")) {
                GameConsole.Execute(_text.text);
                _text.text = string.Empty;
            }
            // Remove the most recent character if Backspace is pressed
            else if (input.Contains("\b")) {
                string currentInput = _text.text;
                if (string.IsNullOrEmpty(currentInput))
                    return;
                _text.text = currentInput.Remove(currentInput.Length - 1);
            }
            // Otherwise extend the current command
            else {
                _text.text += input;
            }
        }
    }
}
