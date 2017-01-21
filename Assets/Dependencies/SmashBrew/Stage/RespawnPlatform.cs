using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.SmashBrew.Characters.Statuses;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("Smash Brew/Stage/Respawn")]
    public class RespawnPlatform : NetworkBehaviour {

        Character _character;

        [SerializeField]
        bool _facing;

        [SerializeField]
        float _invicibilityTimer;

        Invincibility _invincibility;

        [SerializeField]
        float _platformTimer;

        [SyncVar, SerializeField, ReadOnly]
        float _timer;

        [SyncVar, SerializeField, ReadOnly]
        bool _isOccupied;

        public bool Occupied {
            get { return _isOccupied; }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            gameObject.SetActive(false);
            Mediator.Global.Subscribe<PlayerRespawnEvent>(OnEvent);
        }

        void OnDestroy() {
            Mediator.Global.Unsubscribe<PlayerRespawnEvent>(OnEvent);
        }

        void OnEvent(PlayerRespawnEvent eventArgs) {
            if (!isServer || Occupied || eventArgs.Consumed)
                return;
            eventArgs.Consumed = true;
            //TODO(james7132): Fix this
            _character = eventArgs.Player.PlayerObject.GetComponentInChildren<Character>();
            _character.Rigidbody.velocity = Vector3.zero;
            _character.ResetCharacter();
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            Log.Debug(isServer);
            RpcSetActive(true);
            gameObject.SetActive(true);
            var movement = _character.GetComponent<MovementState>();
            if(movement != null)
                movement.RpcMove(transform.position, _facing);
            _isOccupied = true;
        }

        [ClientRpc]
        void RpcSetActive(bool active) {
            gameObject.SetActive(active);
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            if (!isServer || _character == null)
                return;

            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer || (_character.Rigidbody.velocity.magnitude > 0.5f)) {
                _invincibility.Duration -= _platformTimer;
                _character.ResetCharacter();

                RpcSetActive(false);
                gameObject.SetActive(false);
            }
        }

    }

}
