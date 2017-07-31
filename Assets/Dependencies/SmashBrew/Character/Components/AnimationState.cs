using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class AnimationState : CharacterNetworkComponent {

        const int StateLayer = 0;
        const float UpdateRate = 0.2f;

        [SerializeField]
        float _transitionTime = 0.1f;

        [SerializeField]
        Animator _animator;

        Dictionary<int, CharacterState> _states;
        float _updateTimer;

        public Animator Animator {
            get { return _animator; }
            private set { _animator = value; }
        }

        protected override void Awake() {
            base.Awake();
            _updateTimer = 0f;
            Animator = this.CachedGetComponent(Animator, () => GetComponentInChildren<Animator>());
            if (Animator == null)
                throw new InvalidOperationException("No animator found on character: {0}".With(name));
            if (Character != null)
                Character.StateController.OnStateChange += (b, a) => {
                    if (Animator != null)
                        Animator.CrossFade(a.AnimatorHash, 2/60f, 0, _transitionTime);
                };
        }

        void Start() {
            ValidateAnimator();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!hasAuthority)
                return;
            _updateTimer += Time.unscaledDeltaTime;
            if (_updateTimer <= UpdateRate)
                return;
            _updateTimer = 0f;
            var animState = Animator.GetCurrentAnimatorStateInfo(StateLayer);
            CmdChangeState(animState.shortNameHash,  animState.normalizedTime);
        }

        public override void OnStartAuthority() {
            // Update server when the local client has changed.
            Character.StateController.OnStateChange += (b, a) => CmdChangeState(a.AnimatorHash, 0f);
        }

        [Command]
        void CmdChangeState(int animHash, float normalizedTime) {
            //TODO(james7132): Make proper verfications server side
            if (_states == null)
                return;
            if (!_states.ContainsKey(animHash)) {
                Log.Error("Client attempted to set state to one with hash {0}, which has no matching server state.", animHash);
                return;
            }
            RpcChangeState(animHash, normalizedTime);
        }

        [ClientRpc]
        void RpcChangeState(int animHash, float normalizedTime) {
            //TODO(james7132): This gives local players complete control over their networked state. The server should be authoritative on this.
            if (hasAuthority)
                return;
            if (CurrentState.AnimatorHash != animHash) {
                CharacterState newState;
                if (!_states.TryGetValue(animHash, out newState)) {
                    Log.Error("Server attempted to set state to one with hash {0}, which has no matching client state.", animHash);
                } else {
                    Character.StateController.SetState(newState);
                }
            }
            var animState = Animator.GetCurrentAnimatorStateInfo(StateLayer);
            if (animState.shortNameHash != animHash) {
                Log.Debug("HASH MISMATCH: {0} {1}", animState.shortNameHash, animHash);
                Animator.Play(animHash, StateLayer, normalizedTime);
            }
        }

        void ValidateAnimator() {
            if (Character == null)
                return;
            foreach (var state in Character.StateController.States) {
                if (!Animator.HasState(0, state.AnimatorHash))
                    Log.Error("The animator for {0} does not have the state {1} ({2})".With(name, state.Name, state.AnimatorHash));
            }
            _states = Character.StateController.States.ToDictionary(s => s.AnimatorHash);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            if (Animator == null)
                return;
            context.NormalizedAnimationTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public override void ResetState() {
            //TODO(james7132): implement
        }

    }

}

