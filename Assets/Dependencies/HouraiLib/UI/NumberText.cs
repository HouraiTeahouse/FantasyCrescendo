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

using System;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {
    /// <summary> Displays a number to a UnityEngine.UI.Text UI object. </summary>
    [ExecuteInEditMode]
    public class NumberText : MonoBehaviour {
        [SerializeField]
        [Tooltip("The sring format used to display")]
        string _format;

        [SerializeField]
        [Tooltip("The number to display using this script")]
        float _number;

        [SerializeField]
        [Tooltip("The Text UI object driven by this script")]
        Text _text;

        /// <summary> The number to be displayed by the Text UI object. </summary>
        public float Number {
            get { return _number; }
            set {
                bool changed = _number == value;
                _number = value;
                if (!changed)
                    return;
                OnNumberChange.SafeInvoke();
                UpdateText();
            }
        }


        /// <summary> The string format used to display the number. </summary>
        public string Format {
            get { return _format; }
            set {
                bool changed = _format == value;
                _format = value;
                if (changed)
                    UpdateText();
            }
        }

        /// <summary> The Text UI object that is driven by this script. </summary>
        protected Text Text {
            get { return _text; }
            set {
                bool changed = _text == value;
                _text = value;
                if (changed)
                    UpdateText();
            }
        }

        public event Action OnNumberChange;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            if (Text == null)
                Text = GetComponent<Text>();
        }

        /// <summary> Unity Callback. Called when the script is added in the Editor or the Reset menu option is selected. </summary>
        void Reset() {
            Text = GetComponent<Text>();
        }

        /// <summary> Unity Callback. Called once per frame. </summary>
        protected virtual void Update() {
            if (!Application.isPlaying)
                UpdateText();
        }

        protected virtual void UpdateText() {
            if (_text == null)
                return;
            _text.text =
                ProcessNumber(string.IsNullOrEmpty(_format)
                    ? _number.ToString()
                    : _number.ToString(_format));
        }

        protected virtual string ProcessNumber(string number) { return number; }
    }
}
