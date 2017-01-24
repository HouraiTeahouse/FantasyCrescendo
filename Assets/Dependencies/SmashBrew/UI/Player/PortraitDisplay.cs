using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A CharacterUIComponent that displays the portrait of a character on a RawImage object </summary>
    public sealed class PortraitDisplay : CharacterUIComponent<RawImage> {

        [SerializeField]
        [Tooltip("Should the character's portrait be cropped?")]
        bool _cropped;

        Rect _cropRect;

        Color? _defaultColor = null;

        Color DefaultColor {
            get {
                if (_defaultColor == null)
                    _defaultColor = Component.color;
                return _defaultColor.Value;
            }
            set { _defaultColor = value; }
        }

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
            DefaultColor = Component.color;
        }

        // See UIBehaviour.OnRectTransformDimensionsChange
        protected override void OnRectTransformDimensionsChange() { SetRect(); }

        void SetRect() {
            if (_rectTransform == null || Component == null || Component.texture == null)
                return;
            Vector2 size = _rectTransform.rect.size;
            float aspect = size.x / size.y;
            Texture texture = Component.texture;
            Rect imageRect = _cropRect.EnforceAspect(aspect);
            if (imageRect.width > texture.width || imageRect.height > texture.height) {
                imageRect = imageRect.Restrict(texture.width, texture.height, aspect);
                imageRect.center = texture.Center();
            }
            Component.uvRect = texture.PixelToUVRect(imageRect);
        }

        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null || data.PalleteCount <= 0)
                return;
            int portrait = Player != null ? Player.Selection.Pallete : 0;
            data.GetPortrait(portrait).LoadAsync().Then(sprite => {
                if (!sprite)
                    return;
                Texture2D texture = data.GetPortrait(portrait).Asset.texture;
                _cropRect = _cropped ? data.CropRect(texture) : texture.PixelRect();
                _cropRect.position += _rectBias.Mult(texture.Size());
                Component.texture = texture;
                Component.color = data.IsSelectable ? DefaultColor : _disabledTint;
                SetRect();
            }).Done();
        }

    }

}
