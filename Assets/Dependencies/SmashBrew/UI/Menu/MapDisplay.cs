using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// A CharacterUIComponent that displays the portrait of a character on a RawImage object
    /// </summary>
    public sealed class MapDisplay : SceneUIComponent<RawImage> {
        private RectTransform _rectTransform;

        [SerializeField, Tooltip("Should the map's portrait be cropped?")] private bool _cropped;

        [SerializeField, Tooltip("Tint to cover the potrait, should the map be disabled")] private Color
        _disabledTint = Color.gray;

        [SerializeField, Tooltip("An offset to move the crop rect")] private Vector2 _rectBias;

        private Color _defaultColor;
        private Rect _cropRect;

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

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public override void SetData(SceneData data) {
            base.SetData(data);
            if (Component == null || data == null)
                return;
            data.PreviewImage.Load ();
            if (data.PreviewImage == null || data.PreviewImage.Asset == null)
                return;
            Texture2D texture = data.PreviewImage.Asset.texture;
            //_cropRect = _cropped ? data.CropRect(texture) : texture.PixelRect();
            _cropRect = texture.PixelRect();
            _cropRect.x += _rectBias.x * texture.width;
            _cropRect.y += _rectBias.y * texture.height;
            Component.texture = texture;
            //Component.color = data.IsSelectable ? _defaultColor : _disabledTint;
            SetRect();
        }
    }
}