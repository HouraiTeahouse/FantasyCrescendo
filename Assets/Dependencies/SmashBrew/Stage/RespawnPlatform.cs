using System;
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

        void Start() {
            gameObject.SetActive(false);
        }

        public void RespawnPlayer(Character player) {
            if(!player)
                throw new ArgumentNullException("player");
            _character = player;
            _character.Rigidbody.velocity = Vector3.zero;
            _character.transform.position = transform.position;
            _character.transform.rotation = Quaternion.identity;
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            gameObject.SetActive(true);
        }

        // Update is called once per frame
        private void Update() {
            if (_character == null)
                return;

            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (_character.Rigidbody.velocity.magnitude > 0.5f)) {
                _invincibility.Duration -= _platformTimer;
                gameObject.SetActive(false);
            }
        }

    }

}