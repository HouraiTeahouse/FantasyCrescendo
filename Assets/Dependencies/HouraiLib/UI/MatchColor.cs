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

namespace HouraiTeahouse {
    /// <summary> Matches the color between multiple Graphics. </summary>
    [ExecuteInEditMode]
    public class MatchColor : MonoBehaviour {
        [SerializeField]
        Graphic _source;

        [SerializeField]
        Graphic[] _targets;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        void Awake() {
            if (_source == null)
                _source = GetComponent<Graphic>();
        }

        /// <summary> Unity Callback. Called once per frame. </summary>
        void Update() {
            if (_source == null || _targets == null) {
                enabled = false;
                return;
            }

            foreach (Graphic graphic in _targets)
                graphic.color = _source.color;
        }
    }
}