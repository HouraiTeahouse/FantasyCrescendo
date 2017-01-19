using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> UI element that shows where players are </summary>
    [RequireComponent(typeof(Text), typeof(PlayerUIColor))]
    public sealed class PlayerIndicator : PlayerUIComponent {

        [SerializeField]
        CharacterController _characterController;
        // the canvas's RectTransform
        RectTransform _cTransform;

        [SerializeField]
        [Tooltip("Real world position bias for the indicator's position")]
        Vector3 _positionBias = new Vector3(0f, 1f, 0f);

        // the indicator's RectTransform
        RectTransform _rTransform;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.localScale = Vector3.one;
            _cTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        /// <summary> Unity callback. Called once every frame, after all Update calls are processed. </summary>
        void LateUpdate() {
            if (_characterController == null)
                return;
            Bounds bounds = _characterController.bounds;
            Vector3 worldPosition = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + _positionBias;

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this,
            // you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
            //now you can set the position of the ui element
            _rTransform.anchoredPosition = viewportPosition.Mult(_cTransform.sizeDelta) - 0.5f * _cTransform.sizeDelta;
        }

        protected override void PlayerChange() {
            if (Player == null || Player.PlayerObject == null)
                _characterController = null;
            else {
                _characterController = Player.PlayerObject.GetComponent<CharacterController>();
            }
        }

    }

}
