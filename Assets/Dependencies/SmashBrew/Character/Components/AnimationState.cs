using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class AnimationState : CharacterNetworkComponent {

        [SerializeField]
        float _transitionTime = 0.1f;

        [SerializeField]
        Animator _animator;

        public Animator Animator {
            get { return _animator; }
            private set { _animator = value; }
        }

        protected override void Awake() {
            base.Awake();
            if (Animator == null)
                Animator = GetComponentInChildren<Animator>();
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

        void ValidateAnimator() {
            if (Character == null)
                return;
            foreach (var state in Character.StateController.States) {
                if (!Animator.HasState(0, state.AnimatorHash))
                    Log.Error("The animator for {0} does not have the state {1} ({2})".With(name, state.Name, state.AnimatorHash));
            }
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

