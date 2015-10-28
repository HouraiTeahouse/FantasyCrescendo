using UnityEngine;

namespace Hourai.SmashBrew {

    public abstract class CharacterComponent : HouraiBehaviour {
        
        public Character Character { get; private set; }

        protected Animator Animator {
            get {
                if (Character == null)
                    return null;
                return Character.Animator;
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