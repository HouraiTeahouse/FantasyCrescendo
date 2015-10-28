using UnityEngine;
using System;

namespace Hourai.SmashBrew {

    public sealed class Attack : BaseAnimationBehaviour<Character> {

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
        
        [Serializable]
        public class HitboxData {
            public int Bone = -1;
            public Vector3 Offset;
            public float Radius = 1f;
            public float[] TogglePoints;

            private Hitbox hitbox;
            private GameObject gameObject;
            private int index;

            public void Initialize(Character character, Attack parent, int hitboxIndex) {
                gameObject = new GameObject("hb_" + hitboxIndex + "_" + parent._direction + "_" + parent._type + "_" + parent.index);
                Transform hitboxTransform = gameObject.transform;
                hitboxTransform.parent = character.GetBone(Bone);
                hitboxTransform.localPosition = Offset;
                hitboxTransform.localRotation = Quaternion.identity;
                hitboxTransform.localScale = Vector3.one;
                gameObject.AddComponent<SphereCollider>().radius = Radius;
                hitbox = gameObject.AddComponent<Hitbox>();
                gameObject.SetActive(false);
                Array.Sort(TogglePoints);
            }

            public void OnTransition() {
                gameObject.SetActive(false);
                index = -1;
            }

            public void Update(float normalizedTime, Animator animator) {
                if (index >= TogglePoints.Length - 1)
                    return;
                if(normalizedTime > TogglePoints[index + 1]) {
                    index++;
                    gameObject.SetActive(!gameObject.activeSelf);
                }
            }
        }

        [SerializeField]
        private Direction _direction;

        [SerializeField]
        private Type _type;

        [SerializeField]
        private int index;

        [SerializeField]
        private HitboxData[] _data;

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (!Target)
                return;
            for (var i = 0; i < _data.Length; i++)
                _data[i].Initialize(Target, this, i);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target)
               Target.Attack(_type, _direction, index);

            Transition();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            float t = stateInfo.normalizedTime;
            for (var i = 0; i < _data.Length; i++)
                _data[i].Update(t, animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Transition();
        }

        void Transition() {
            for (var i = 0; i < _data.Length; i++)
                _data[i].OnTransition();
        }
    }
}