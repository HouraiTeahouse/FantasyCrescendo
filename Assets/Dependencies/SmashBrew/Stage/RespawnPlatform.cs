using UnityEngine;

namespace Hourai.SmashBrew {

    public class RespawnPlatform : HouraiBehaviour {

        [SerializeField]
        private float _invicibilityTimer;

        [SerializeField]
        private float _platformTimer;

        private Character _character;
        private Invincibility _invincibility;
        private float _timer;

        public Character Character {
            get { return _character; }
            set {
                if (value == null)
                    return;

                _character = value;
                if (!_character)
                    return;

                _character.Rigidbody.velocity = Vector3.zero;
                _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            }
        }

        // Update is called once per frame
        private void Update() {
            if (_character == null)
                return;

            _timer += Util.dt;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (Character.Rigidbody.velocity.magnitude > 0.5f)) {
                _invincibility.Duration -= _platformTimer;
                Destroy(gameObject);
            }
        }

    }

}