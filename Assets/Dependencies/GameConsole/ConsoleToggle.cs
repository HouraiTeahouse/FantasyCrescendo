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
using UnityEngine.EventSystems;

namespace HouraiTeahouse.Console {
    /// <summary> UI Element to toggle the appearance of the GameConsole UI. </summary>
    public class ConsoleToggle : MonoBehaviour {

        [SerializeField]
        [Tooltip("The KeyCode for the key that toggles the appearance of the GameConsole UI.")]
        KeyCode _key = KeyCode.F5;

        [SerializeField]
        [Tooltip("GameObjects to activate and deactivate when toggling the GameConsole UI.")]
        GameObject[] _toggle;

        [SerializeField]
        [Tooltip("GameObject to selet when GameConsole is set to show")]
        GameObject select;

        /// <summary> Unity callback. Called once every frame. </summary>
        void Update() {
            if (!Input.GetKeyDown(_key))
                return;
            foreach (GameObject go in _toggle) {
                if (!go)
                    continue;
                go.SetActive(!go.activeSelf);
            }
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem)
                eventSystem.SetSelectedGameObject(select);
        }
    }
}
