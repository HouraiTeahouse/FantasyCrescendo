using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    public class RespawnPlatform : HouraiBehaviour {

        [Serialize]
        private float _invicibilityTimer;

        [Serialize]
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
                if (_character != null) {
                    _character.Velocity = Vector3.zero;
                    _invincibility = _character.AddStatus<Invincibility>(_invicibilityTimer + _platformTimer);
                }
            }
        }

        // Update is called once per frame
        private void Update() {
            if (_character == null)
                return;

            _timer += Util.dt;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (Character.Velocity.magnitude > 0.5f)) {
                _invincibility.Duration -= _platformTimer;
                Destroy(gameObject);
            }
        }

    }

}