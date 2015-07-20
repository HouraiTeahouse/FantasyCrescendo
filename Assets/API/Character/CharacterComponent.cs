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
            if (Character == null) {
                enabled = false;
                Debug.LogWarning(GetType() + " on " + name + " has not found a suitable Character component. Please attach one.");   
            }
        }

        protected void Update() {
            if (Character == null) {
                enabled = false;
                return;
            }
            OnUpdate();
        }

        protected virtual void OnUpdate() {
        }

    }

}