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

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> UI element that shows where players are </summary>
    [RequireComponent(typeof(Text), typeof(PlayerUIColor))]
    public sealed class PlayerIndicator : PlayerUIComponent {
        CapsuleCollider _collider;
        // the canvas's RectTransform
        RectTransform _cTransform;

        [SerializeField]
        [Tooltip("Real world position bias for the indicator's position")]
        Vector3 _positionBias = new Vector3(0f, 1f, 0f);

        // the indicator's RectTransform
        RectTransform _rTransform;

        Player _target;

        /// <summary> The Player for the PlayerIndicator to follow. </summary>
        public Player Target {
            get { return _target; }
            set { SetData(value); }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.localScale = Vector3.one;
            _cTransform =
                GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        /// <summary> Unity callback. Called once every frame, after all Update calls are processed. </summary>
        void LateUpdate() {
            if (Target == null)
                return;
            Bounds bounds = _collider.bounds;
            Vector3 worldPosition = bounds.center
                + new Vector3(0f, bounds.extents.y, 0f) + _positionBias;

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this,
            // you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition =
                Camera.main.WorldToViewportPoint(worldPosition);
            //now you can set the position of the ui element
            _rTransform.anchoredPosition =
                ViewportPosition.Mult(_cTransform.sizeDelta)
                    - 0.5f * _cTransform.sizeDelta;
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public override void SetData(Player data) {
            base.SetData(data);
            _target = data;
            _collider = _target != null
                ? _target.PlayerObject.MovementCollider
                : null;
        }
    }
}
