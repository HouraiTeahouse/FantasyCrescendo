using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// UI element that shows where players are
    /// </summary>
    [RequireComponent(typeof (Text), typeof (PlayerUIColor))]
    public sealed class PlayerIndicator : PlayerUIComponent {
        [SerializeField, Tooltip("Real world position bias for the indicator's position")] private Vector3 _positionBias
            = new Vector3(0f, 1f, 0f);

        // the indicator's RectTransform
        private RectTransform _rTransform;
        // the canvas's RectTransform
        private RectTransform _cTransform;

        private Player _target;
        private CapsuleCollider _collider;

        /// <summary>
        /// The Player for the PlayerIndicator to follow.
        /// </summary>
        public Player Target {
            get { return _target; }
            set { SetData(value); }
        }

        /// <summary>
        /// Unity callback. Called on object instantiation.  
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.localScale = Vector3.one;
            _cTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        /// <summary>
        /// Unity callback. Called once every frame, after all Update calls are processed.
        /// </summary>
        void LateUpdate() {
            if (Target == null)
                return;
            Bounds bounds = _collider.bounds;
            Vector3 worldPosition = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + _positionBias;

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this,
            // you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
            //now you can set the position of the ui element
            _rTransform.anchoredPosition = ViewportPosition.Mult(_cTransform.sizeDelta) - 0.5f * _cTransform.sizeDelta;
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public override void SetData(Player data) {
            base.SetData(data);
            _target = data;
            _collider = (_target != null) ? _target.PlayerObject.MovementCollider : null;
        }
    }
}