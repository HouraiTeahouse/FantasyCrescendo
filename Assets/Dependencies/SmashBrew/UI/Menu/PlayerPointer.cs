using InControl;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerPointer : PlayerUIComponent {

        private RectTransform _rTransform;
        
        [SerializeField]
        private float _movementSpeed = 20;

        [SerializeField]
        private InputControlTarget _horizontal = InputControlTarget.LeftStickX;

        [SerializeField]
        private InputControlTarget _vertical = InputControlTarget.LeftStickY;

        private InputDevice _input;

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            _input = Player != null ? Player.Controller : null;
        }

        protected override void Awake() {
            base.Awake();
            _rTransform = transform as RectTransform;
        }

        // Update is called once per frame
        void Update() {
            if (_input == null)
                return;
            Vector2 movement = new Vector2(_input.GetControl(_horizontal), _input.GetControl(_vertical));
            if(!Player.IsActive && movement != Vector2.zero)
                Player.CycleType();
            Bounds bounds = new Bounds(Vector3.zero, (_rTransform.parent as RectTransform).rect.size - 0.5f * _rTransform.sizeDelta);
            _rTransform.anchoredPosition = bounds.ClosestPoint(_rTransform.anchoredPosition + _movementSpeed * movement * Time.deltaTime);
        }
    }

}
