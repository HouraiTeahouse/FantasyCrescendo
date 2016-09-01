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
using UnityEngine.UI;

namespace HouraiTeahouse {

    public abstract class UITextBehaviour : UIBehaviour {

        string _displayedText;

        [SerializeField]
        string _format;

        [SerializeField]
        Text _text;

        /// <summary>
        /// The string value to display on the text.
        /// </summary>
        protected virtual string DisplayedText {
            get { return _displayedText; }
            set {
                _displayedText = value;
                if (_text)
                    _text.text = !_format.IsNullOrEmpty()
                        ? _format.With(value)
                        : value;
            }
        }

        /// <summary>
        ///   <para>See MonoBehaviour.Awake.</para>
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_text)
                _text = GetComponent<Text>();
        }
    }

    public sealed class VersionText : UITextBehaviour {
        protected override void Awake() {
            base.Awake();
            DisplayedText = Application.version;
        }
    }
}
