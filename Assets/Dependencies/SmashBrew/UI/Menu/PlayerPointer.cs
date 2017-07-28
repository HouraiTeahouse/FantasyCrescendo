using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerPointer : PlayerUIComponent {

        CharacterSelectInputModule _inputModule;

        [SerializeField]
        float _movementSpeed = 20;

        RectTransform _rectTransform;

        protected override void Start() {
            base.Start();
            _rectTransform = transform as RectTransform;
            _inputModule = CharacterSelectInputModule.Instance;
            if (!_inputModule) {
                Destroy(this);
                return;
            }
            _inputModule.AddPointer(this);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_inputModule)
                _inputModule.RemovePointer(this);
        }

        public void Move(Vector2 movement) {
            if (!Player.Type.IsActive && movement != Vector2.zero)
                Player.CycleType();
            var bounds = new Bounds(Vector3.zero,
                ((RectTransform) _rectTransform.parent).rect.size - 0.5f * _rectTransform.sizeDelta);
            _rectTransform.anchoredPosition =
                bounds.ClosestPoint(_rectTransform.anchoredPosition + _movementSpeed * movement * Time.deltaTime);
        }

    }

}