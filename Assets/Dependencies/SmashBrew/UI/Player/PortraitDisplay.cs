using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// A CharacterUIComponent that displays the portrait of a character on a RawImage object
    /// </summary>
    public sealed class PortraitDisplay : CharacterUIComponent<RawImage> {

        private RectTransform _rectTransform;

        [SerializeField, Tooltip("Should the character's portrait be cropped?")]
        private bool _cropped;

        [SerializeField, Tooltip("Tint to cover the potrait, should the character be disabled")]
        private Color _disabledTint = Color.gray;

        [SerializeField, Tooltip("An offset to move the crop rect")]
        private Vector2 _rectBias;

        private Color _defaultColor;
        private Rect _cropRect;
        private float _aspectRatio;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _rectTransform = Component.GetComponent<RectTransform>();
            _defaultColor = Component.color;
        }

        /// <summary>
        /// <see cref="UIBehaviour.OnRectTransformDimensionsChange"/>
        /// </summary>
        protected override void OnRectTransformDimensionsChange() {
            SetRect(); 
        }

        void SetRect() {
            if (_rectTransform == null)
                return;
            Vector2 size = _rectTransform.rect.size;
            float aspect = size.x / size.y;
            Rect imageRect = _cropRect;
            float diff, dim;
            if (aspect > _aspectRatio) {
                // Image is wider than cropRect, extend it sideways
                imageRect.width = aspect * imageRect.height;
                imageRect.center = _cropRect.center;
            } else {
                // Image is wider than cropRect, extend it vertically
                imageRect.height = imageRect.width / aspect;
                imageRect.center = _cropRect.center;
            }
            if (imageRect.width > 1) {
                imageRect.width = 1;
                imageRect.height = 1 / aspect;
                imageRect.center = 0.5f * Vector2.one;
            } else if (imageRect.height > 1) {
                imageRect.width = aspect;
                imageRect.height = 1;
                imageRect.center = 0.5f * Vector2.one;
            }
            Component.uvRect = imageRect;
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null || data.PalleteCount <= 0)
                return;
            int portrait = Player != null ? Player.Pallete : 0;
            if (data.GetPortrait(portrait).Load() == null)
                return;
            Texture2D texture = data.GetPortrait(portrait).Asset.texture;
            _cropRect = _cropped ? data.CropRect(texture) : new Rect(0, 0, 1, 1);
            _aspectRatio = _cropRect.width / _cropRect.height;
            _cropRect.x += _rectBias.x;
            _cropRect.y += _rectBias.y;
            Component.texture = texture;
            Component.color = data.IsSelectable ? _defaultColor : _disabledTint;
            SetRect();
        }

    }
}

