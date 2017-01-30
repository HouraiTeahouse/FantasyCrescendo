using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [SharedBetweenAnimators]
    public sealed class CharacterStateEvents : BaseAnimationBehaviour<Character> {

        [SerializeField]
        AnimationClip _clip;

        [SerializeField]
        string _stateName;

        [SerializeField]
        EventData _eventData;

        bool initalized = false;

        public EventData Data {
            get { return _eventData; }
            set { _eventData = value; }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            ResetHitboxes(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            ResetHitboxes(animator);
        }

        void ResetHitboxes(Animator animator) {
            var character = animator.GetComponentInParent<Character>();
            if (character == null)
                return;
            character.ResetAllHitboxes();
        }

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (initalized)
                return;
            if (_clip == null || _eventData == null) {
                Log.Error("State {0} does not have a clip. Is it initialized?", _stateName);
                return;
            }
            _clip.events = _eventData.GetEvents().ToArray();
            initalized = true;
            Log.Info("[Animation/Hitbox] Initialized {0} on {1}. Added {2} events, {3} hitbox keyframes.",
                _stateName,
                gameObject.name,
                _clip.events.Length,
                _eventData.Keyframes.Count);
        }

    }

}
