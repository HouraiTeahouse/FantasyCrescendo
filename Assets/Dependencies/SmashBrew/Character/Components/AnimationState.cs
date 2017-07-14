using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementState))]
    [RequireComponent(typeof(PhysicsState))]
    public class AnimationState : CharacterNetworkComponent {

        [SerializeField]
        float _transitionTime = 0.1f;

        MovementState Movement { get; set; }
        PhysicsState Physics { get; set; }
        CharacterController CharacterController { get; set; }

        public Animator Animator {
            get { return Character != null ? Character.Animator : null; }
        }

        protected override void Awake() {
            base.Awake();
            Movement = this.SafeGetComponent<MovementState>();
            Physics = this.SafeGetComponent<PhysicsState>();
            CharacterController = this.SafeGetComponent<CharacterController>();
            if (Character != null)
                Character.StateController.OnStateChange += (b, a) => {
                    Log.Debug(_transitionTime);
                    if (Animator != null)
                        Animator.CrossFade(a.AnimatorHash, 2/60f, 0, _transitionTime);
                };
        }

        float Sign(float x) {
            if (x > 0)
                return 1;
            if (x < 0)
                return -1;
            return 0;
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

