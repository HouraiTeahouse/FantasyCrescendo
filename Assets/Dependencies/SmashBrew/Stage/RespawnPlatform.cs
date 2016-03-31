using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public class RespawnPlatform : EventHandlerBehaviour<PlayerRespawnEvent> {
        [SerializeField] private float _invicibilityTimer;

        [SerializeField] private float _platformTimer;

        [SerializeField] private bool _facing;

        public bool Occupied {
            get { return _character; }
        }

        private Character _character;
        private Invincibility _invincibility;
        private float _timer;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            gameObject.SetActive(false);
        }

        protected override void OnEvent(PlayerRespawnEvent eventArgs) {
            if (Occupied || eventArgs.Consumed)
                return;
            eventArgs.Consumed = true;
            _character = eventArgs.Player.PlayerObject;
            _character.Rigidbody.velocity = Vector3.zero;
            _character.transform.position = transform.position;
            _character.Direction = _facing;
			_character.ResetCharacter ();
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        void Update() {
            if (_character == null)
                return;

            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (_character.Rigidbody.velocity.magnitude > 0.5f)) {
                _invincibility.Duration -= _platformTimer;
				_character.ResetCharacter ();

                gameObject.SetActive(false);
            }
        }
    }
}