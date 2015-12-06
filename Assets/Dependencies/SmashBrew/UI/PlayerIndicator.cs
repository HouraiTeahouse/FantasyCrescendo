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
        private PlayerUiColor _cUIColor;
        private Text _text;

        private Player _target;
        private CapsuleCollider _collider;

        public Player Target {
            get { return _target; }
            set {
                _target = value;
                _collider = (_target != null)? _target.SpawnedCharacter.MovementCollider : null;
            }
        }

        private void Awake() {
            GameObject gui = Game.FindGUI();
            if(!gui)
                Destroy(this);
            _text = GetComponent<Text>();
            _cUIColor = GetComponent<PlayerUiColor>();
            _rTransform = GetComponent<RectTransform>();
            _rTransform.SetParent(gui.transform);
            _rTransform.localScale = Vector3.one;
        }

        private void LateUpdate() {
            if (Target == null) {
                _text.enabled = false;
                return;
            }

            _text.text = (Target.PlayerNumber + 1).ToString(_format);
            _cUIColor.SetPlayerData(Target);

            Bounds bounds = _collider.bounds;
            Vector3 worldPosition = bounds.center + new Vector3(0f, bounds.extents.y, 0f) + _positionBias;
            _rTransform.position = Camera.main.WorldToScreenPoint(worldPosition);
        }

    }
}