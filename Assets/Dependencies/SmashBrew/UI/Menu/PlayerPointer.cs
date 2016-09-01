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
    public class PlayerPointer : PlayerUIComponent {
        CharacterSelectInputModule _inputModule;

        [SerializeField]
        float _movementSpeed = 20;

        RectTransform _rectTransform;

        protected override void Start() {
            base.Start();
            _rectTransform = transform as RectTransform;
            _inputModule = CharacterSelectInputModule.Instance;
            if (!_inputModule) {
                Destroy(this);
                return;
            }
            _inputModule.AddPointer(this);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_inputModule)
                _inputModule.RemovePointer(this);
        }

        public void Move(Vector2 movement) {
            if (!Player.Type.IsActive && movement != Vector2.zero)
                Player.CycleType();
            var bounds = new Bounds(Vector3.zero,
                ((RectTransform) _rectTransform.parent).rect.size
                    - 0.5f * _rectTransform.sizeDelta);
            _rectTransform.anchoredPosition =
                bounds.ClosestPoint(_rectTransform.anchoredPosition
                    + _movementSpeed * movement * Time.deltaTime);
        }
    }
}
