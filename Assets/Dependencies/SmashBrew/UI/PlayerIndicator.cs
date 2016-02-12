using UnityConstants;
using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Text), typeof(PlayerUiColor))]
    public sealed class PlayerIndicator : MonoBehaviour {

        [SerializeField]
        private Vector3 _positionBias = new Vector3(0f, 1f, 0f);

        [SerializeField]
        private string _format;

        private RectTransform _rTransform;
        private RectTransform _cTransform;
        private PlayerUiColor _cUIColor;
        private Text _text;

        private Player _target;
        private CapsuleCollider _collider;

        public Player Target {
            get { return _target; }
            set {
                _target = value;
                _collider = (_target != null)? _target.SpawnedCharacter.MovementCollider : null;
                if (_target != null)
                    _text.text = (_target.PlayerNumber + 1).ToString(_format);
                _cUIColor.SetPlayerData(_target);
            }
        }

        private void Awake() {
            GameObject gui = GameObject.FindGameObjectWithTag(Tags.GUI); 
            if(!gui)
                Destroy(this);
            _text = GetComponent<Text>();
            _cUIColor = GetComponent<PlayerUiColor>();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.SetParent(gui.transform);
            _rTransform.localScale = Vector3.one;
            _cTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        private void LateUpdate() {
            if (Target == null) {
                _text.enabled = false;
                return;
            }
            Bounds bounds = _collider.bounds;
            Vector3 worldPosition = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + _positionBias;

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * _cTransform.sizeDelta.x) - (_cTransform.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * _cTransform.sizeDelta.y) - (_cTransform.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            _rTransform.anchoredPosition = WorldObject_ScreenPosition;
        }
    }
}
