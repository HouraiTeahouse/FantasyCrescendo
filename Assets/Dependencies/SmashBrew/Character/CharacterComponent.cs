using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    [RequireComponent(typeof (Character))]
    public abstract class CharacterComponent : HouraiBehaviour {

        [DontSerialize, Hide]
        public Character Character { get; private set; }

        protected Animator Animator {
            get {
                if (Character == null)
                    return null;
                return Character.Animator;
            }
        }

        protected ICharacterInput InputSource {
            get { return Character == null ? null : Character.InputSource; }
            set {
                if (Character == null)
                    return;
                Character.InputSource = value;
            }
        }

        protected virtual void Start() {
            Character = GetComponentInParent<Character>();
            if (Character == null) {
                enabled = false;
                Debug.LogWarning(GetType() + " on " + name +
                                 " has not found a suitable Character component. Please attach one.");
            }
        }

        protected void Update() {
            if (Character == null) {
                enabled = false;
                return;
            }
            OnUpdate();
        }

        protected virtual void OnUpdate() {}

    }

}