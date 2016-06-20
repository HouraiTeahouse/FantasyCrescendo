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

using System.Collections;
using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.UI;
using UnityEngine;

namespace HouraiTeahouse {
    public class SpawnPlayerPointer : PlayerUIComponent {
        RectTransform _cTransform;
        RectTransform _currentPointer;

        [SerializeField]
        RectTransform _pointer;

        [SerializeField]
        [Tag]
        string _tag;

        /// <summary> Unity callback. Called on object instaniation. </summary>
        protected override void Awake() {
            base.Awake();
            GameObject go = GameObject.FindWithTag(_tag);
            _currentPointer = Instantiate(_pointer);
            _currentPointer.SetParent(go.transform);
            _cTransform =
                _currentPointer.GetComponentInParent<Canvas>().transform as
                    RectTransform;
            StartCoroutine(Test());
        }

        // Ridiculously hacky coroutine to get the pointer to the right location 
        IEnumerator Test() {
            // need to wait two frames frames before everything is properly settled
            yield return null;
            yield return null;
            Vector2 viewportPos =
                Camera.main.WorldToViewportPoint(transform.position);
            _currentPointer.anchoredPosition3D =
                viewportPos.Mult(_cTransform.sizeDelta)
                    - 0.5f * _cTransform.sizeDelta;
            _currentPointer.localScale = Vector3.one;
        }

        /// <summary> </summary>
        protected override void OnDestroy() {
            base.OnDestroy();
            if (_currentPointer)
                Destroy(_currentPointer.gameObject);
        }

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (_currentPointer)
                _currentPointer.GetComponentsInChildren<IDataComponent<Player>>()
                    .SetData(Player);
        }
    }
}
