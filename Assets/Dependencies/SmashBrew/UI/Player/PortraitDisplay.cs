using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PortraitDisplay : CharacterUIComponent<RawImage> {

        private RectTransform _rectTransform;

        [SerializeField]
        private bool _cropped;

        [SerializeField]
        private Color _disabledTint = Color.gray;

        [SerializeField]
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

        protected override void OnRectTransformDimensionsChange() {
            if (_rectTransform == null)
                return;
            Vector2 size = _rectTransform.rect.size;
            float aspect = size.x / size.y;
            Rect imageRect = _cropRect;
            float diff, dim;
            if (aspect > _aspectRatio) {
                // Image is wider than cropRect, extend it sideways
                dim = aspect * imageRect.height;
                diff = imageRect.width - dim;
                imageRect.width = dim;
                imageRect.x += 0.5f * diff;
                ;
            } else {
                // Image is wider than cropRect, extend it vertically 
                dim = imageRect.width / aspect;
                diff = imageRect.height - dim;
                imageRect.height = dim;
                imageRect.y += 0.5f * diff;
            }
            Component.uvRect = imageRect;
        }

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
            Component.uvRect = _cropRect;
            Component.color = data.IsSelectable ? _defaultColor : _disabledTint;
        }

    }
}

