using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class RespawnPlatform : HouraiBehaviour {

        [SerializeField]
        private float _invicibilityTimer;

        [SerializeField]
        private float _platformTimer;

        public bool Occupied {
            get { return _character; }
        }

        private Character _character;
        private Invincibility _invincibility;
        private float _timer;
        private Mediator eventManager;

        protected override void Awake() {
            base.Awake();
            gameObject.SetActive(false);
            eventManager = GlobalMediator.Instance;
            eventManager.Subscribe<RespawnEvent>(RespawnPlayer);
        }

        void RespawnPlayer(RespawnEvent eventArgs) {
            if (Occupied || eventArgs.Consumed)
                return;
            eventArgs.Consumed = true;
            _character = eventArgs.Player.PlayerObject;
            _character.Rigidbody.velocity = Vector3.zero;
            _character.transform.position = transform.position;
            _character.transform.rotation = transform.localRotation;
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update() {
            if (_character == null)
                return;

            Debug.Log(_character);
            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (_character.Rigidbody.velocity.magnitude > 0.5f)) {
                Debug.Log(_timer + " " + _character.Rigidbody.velocity);
                _invincibility.Duration -= _platformTimer;
                gameObject.SetActive(false);
            }
        }

    }

}
