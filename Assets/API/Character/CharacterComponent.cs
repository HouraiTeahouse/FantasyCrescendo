using UnityEngine;

namespace Genso.API {

    [RequireComponent(typeof(Character))]
    public abstract class CharacterComponent : GensoBehaviour {

        protected Character Character { get; private set; }

        protected ICharacterInput InputSource {
            get {
                return Character == null ? null : Character.InputSource;
            }
            set {
                if (Character == null)
                    return;
                Character.InputSource = value;
            }
        }

        protected virtual void Awake() {
            Character = GetComponentInParent<Character>();
            if(Character != null)
                Character.AddCharacterComponent(this);
            else
                Debug.LogWarning(GetType() + " on " + name + " has not found a suitable Character component. Please attach one.");
        }

        protected virtual void OnDestroy() {
            if (Character == null)
                return;
            Character.RemoveCharacterComponent(this);
        }

        public virtual void OnMove(Vector2 direction) {
        }

        public virtual void OnHelpless() {
        }

        public virtual void OnJump() {
        }

        public virtual void OnGrounded() {
        }

        public virtual void OnBlastZoneExit() {
        }

    }

}