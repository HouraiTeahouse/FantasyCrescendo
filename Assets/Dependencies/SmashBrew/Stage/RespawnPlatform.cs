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

        [SyncVar(hook = "OccupationChanged"), SerializeField, ReadOnly]
        bool _isOccupied;

        public bool Occupied {
            get { return _isOccupied; }
            private set { OccupationChanged(value); }
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            Mediator.Global.Subscribe<PlayerRespawnEvent>(OnEvent);
        }

        void OnDestroy() {
            Mediator.Global.Unsubscribe<PlayerRespawnEvent>(OnEvent);
        }

        public override void OnStartServer() {
            gameObject.SetActive(false);
            _isOccupied = false;
        }

        public override void OnStartClient() {
            gameObject.SetActive(_isOccupied);
        }

        void OccupationChanged(bool isOccupied) {
            _isOccupied = isOccupied;
            gameObject.SetActive(_isOccupied);
        }

        void OnEvent(PlayerRespawnEvent eventArgs) {
            if (Occupied || eventArgs.Consumed)
                return;
            eventArgs.Consumed = true;
            //TODO(james7132): Fix this
            _character = eventArgs.Player.PlayerObject.GetComponentInChildren<Character>();
            _character.Movement.RpcMove(transform.position);
            _character.ResetCharacter();
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            eventArgs.Player.PlayerObject.SetActive(true);
            Occupied = true;
            _isOccupied = true;
        }

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            if (!isServer || _character == null)
                return;

            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer) {
                _invincibility.Duration -= _platformTimer;
                _character.ResetCharacter();

                Occupied = false;
                gameObject.SetActive(false);
            }
        }

    }

}
