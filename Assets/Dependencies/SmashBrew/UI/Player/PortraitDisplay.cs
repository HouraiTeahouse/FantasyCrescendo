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

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A CharacterUIComponent that displays the portrait of a character on a RawImage object </summary>
    public sealed class PortraitDisplay : CharacterUIComponent<RawImage> {
        [SerializeField]
        [Tooltip("Should the character's portrait be cropped?")]
        bool _cropped;

        Rect _cropRect;

        Color _defaultColor;

        [SerializeField]
        [Tooltip("Tint to cover the potrait, should the character be disabled")]
        Color _disabledTint = Color.gray;

        [SerializeField]
        [Tooltip("An offset to move the crop rect")]
        Vector2 _rectBias;

        RectTransform _rectTransform;

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _rectTransform = Component.GetComponent<RectTransform>();
            _defaultColor = Component.color;
        }

        /// <summary>
        ///     <see cref="UIBehaviour.OnRectTransformDimensionsChange" />
        /// </summary>
        protected override void OnRectTransformDimensionsChange() {
            SetRect();
        }

        void SetRect() {
            if (_rectTransform == null || Component == null
                || Component.texture == null)
                return;
            Vector2 size = _rectTransform.rect.size;
            float aspect = size.x / size.y;
            Texture texture = Component.texture;
            Rect imageRect = _cropRect.EnforceAspect(aspect);
            if (imageRect.width > texture.width
                || imageRect.height > texture.height) {
                imageRect = imageRect.Restrict(texture.width,
                    texture.height,
                    aspect);
                imageRect.center = texture.Center();
            }
            Component.uvRect = texture.PixelToUVRect(imageRect);
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null || data.PalleteCount <= 0)
                return;
            int portrait = Player != null ? Player.Pallete : 0;
            data.GetPortrait(portrait).LoadAsync(delegate(Sprite sprite) {
                if (!sprite)
                    return;
                Texture2D texture = data.GetPortrait(portrait).Asset.texture;
                _cropRect = _cropped
                    ? data.CropRect(texture)
                    : texture.PixelRect();
                _cropRect.x += _rectBias.x * texture.width;
                _cropRect.y += _rectBias.y * texture.height;
                Component.texture = texture;
                Component.color = data.IsSelectable
                    ? _defaultColor
                    : _disabledTint;
                SetRect();
            });
        }
    }
}
