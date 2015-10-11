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
            Character.AddCharacterComponent(this);
        }

        protected virtual void OnDestroy() {
            if(Character)
                Character.RemoveCharacterComponent(this);
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