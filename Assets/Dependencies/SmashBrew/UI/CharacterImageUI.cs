using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(RawImage))]
    public class CharacterImageUI : UIBehaviour, ICharacterGUIComponent {

        private RawImage _image;
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
            _image = GetComponent<RawImage>();
            _defaultColor = _image.color;
            _rectTransform = transform as RectTransform;
        }

        protected override void OnRectTransformDimensionsChange() {
            Vector2 size = _rectTransform.rect.size;
            float aspect = size.x / size.y;
            Rect imageRect = _cropRect;
            float diff, dim;
            if (aspect > _aspectRatio) {
                Debug.Log("Big");
                // Image is wider than cropRect, extend it vertically 
                dim = aspect * imageRect.width;
                diff = imageRect.height - dim;
                imageRect.height = dim;
                imageRect.y -= 0.5f * diff;
            } else {
                Debug.Log("Small");
                // Image is wider than cropRect, extend it sideways
                dim = aspect * imageRect.height;
                diff = imageRect.width - dim;
                imageRect.width = dim;
                imageRect.x += 0.5f * diff;
            }
            _image.uvRect = imageRect;
        }

        public void SetCharacter(CharacterData character) {
            if (!character || character.AlternativeCount <= 0 || character.GetPortrait(0).Load() == null)
                return;
            _image.texture = character.GetPortrait(0).Asset.texture;
            _cropRect = _cropped ? character.CropRect(_image.texture) : new Rect(0,0,1,1);
            _aspectRatio = _cropRect.width / _cropRect.height;
            _cropRect.x += _rectBias.x;
            _cropRect.y += _rectBias.y;
            _image.uvRect = _cropRect;
            _image.color = character.IsSelectable ? _defaultColor : _disabledTint;
        }

    }

}
