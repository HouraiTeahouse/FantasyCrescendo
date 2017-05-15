using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> UI element that shows where players are </summary>
    [RequireComponent(typeof(Text), typeof(PlayerUIColor))]
    public sealed class PlayerIndicator : PlayerUIComponent<Graphic> {

        CharacterController _characterController;
        Transform _trackingTarget;
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
            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this,
            // you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(GetTargetPosition());
            //now you can set the position of the ui element
            _rTransform.anchoredPosition = viewportPosition.Mult(_cTransform.sizeDelta) - 0.5f * _cTransform.sizeDelta;

            if (Component)
                Component.enabled = _characterController.gameObject.activeInHierarchy;
        }

        Vector3 GetTargetPosition() {
            var position = Vector3.zero;
            if (_trackingTarget != null)
                position = _trackingTarget.position;
            else if (_characterController != null) {
                Bounds bounds = _characterController.bounds;
                position = bounds.center + new Vector3(0f, bounds.extents.y, 0f);
            } 
            return position + _positionBias;
        }

        protected override void PlayerChange() {
            if (Player == null || Player.PlayerObject == null)
                _characterController = null;
            else {
                _characterController = Player.PlayerObject.GetComponent<CharacterController>();
                _trackingTarget = Player.PlayerObject.GetComponentsInChildren<Transform>()
                    .FirstOrDefault(t => t.CompareTag("Indicator Tracker"));
            }
        }

    }

}
