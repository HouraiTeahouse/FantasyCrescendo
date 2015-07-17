using UnityEngine;
using System.Collections;

namespace Genso.API {

    [RequireComponent(typeof(Character))]
    public abstract class CharacterComponent : GensoBehaviour {

        protected Character Character { get; private set; }

        protected virtual void Awake() {
            Character = GetComponentInParent<Character>();
            Character.AddCharacterComponent(this);
        }

        protected virtual void OnDestroy() {
            Character.RemoveCharacterComponent(this);
        }

        public virtual void OnJump() {
        }

        public virtual void OnGrounded() {
        }

    }

}