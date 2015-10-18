using UnityEngine;
using System;

namespace Hourai.SmashBrew {

    public sealed class Attack : CharacterAnimationBehaviour {

        public enum Type {
            Normal,
            Smash,
            Aerial,
            Special
        }

        public enum Direction {
            Neutral,
            Up,
            Front,
            Back,
            Down
        }

        [SerializeField]
        private Direction _direction;

        [SerializeField]
        private Type _type;

        [SerializeField]
        private AttackData _data;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (!Character) {
                Destroy(this);
                return;
            }
            // Set any events
            Character.Attack(_type, _direction);
            if (_data)
                _data.Transition();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (_data)
                _data.Update(animator, stateInfo);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (_data)
                _data.Transition();
        }

    }

    public class AttackData : ScriptableObject {

        [SerializeField]
        private Avatar avatar;

        [Serializable]
        public class HitboxData {
            public int bone;
            public Vector3 Offset;
            public float Radius;
            public float Start;
            public float End;

            private Hitbox hitbox;

            public void OnTransition() {
                if (hitbox)
                    Destroy(hitbox.gameObject);
            }

            public void Update(float normalizedTime, Animator animator) {
                if(hitbox) {
                    if (normalizedTime > End)
                        Destroy(hitbox.gameObject);
                } else {
                    if (normalizedTime > Start && normalizedTime < End)
                        hitbox = CreateHitbox();
                }
            }

            Hitbox CreateHitbox() {
                // TODO: Implement
                return null;
            }

        }

        [SerializeField]
        private HitboxData[] _hitboxes;

        public void Update(Animator animator, AnimatorStateInfo stateInfo) {
            float t = stateInfo.normalizedTime;
            for (var i = 0; i < _hitboxes.Length; i++)
                _hitboxes[i].Update(t, animator);
        }

        public void Transition() {
            for (var i = 0; i < _hitboxes.Length; i++)
                _hitboxes[i].OnTransition();
        }
    }
}
