using UnityEngine;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    [RequireComponent(typeof(Text), typeof(PlayerUiColor))]
    public sealed class PlayerIndicator : MonoBehaviour {

        [SerializeField]
        private Vector3 positionBias = new Vector3(0f, 1f, 0f);

        [SerializeField]
        private string _format;

        private RectTransform _rTransform;
        private PlayerUiColor _cUIColor;
        private Text _text;

        private Character _target;
        private CapsuleCollider _collider;

        public Character Target {
            get { return _target; }
            set {
                _target = value;
                _collider = _target ? _target.MovementCollider : null;
            }
        }

        private void Awake() {
            _text = GetComponent<Text>();
            _cUIColor = GetComponent<PlayerUiColor>();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.SetParent(SmashGame.FindGUI().transform);
            _rTransform.localScale = Vector3.one;
        }

        private void LateUpdate() {
            if (Target == null) {
                _text.enabled = false;
                return;
            }

            _text.text = (Target.PlayerNumber + 1).ToString(_format);
            _cUIColor.SetPlayerData(Match.GetPlayerData(Target.PlayerNumber));

            Bounds bounds = _collider.bounds;
            Vector3 worldPosition = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + positionBias;
            _rTransform.position = CameraController.Camera.WorldToScreenPoint(worldPosition);
            
        }

    }
}