using UnityEngine;

namespace Hourai.SmashBrew {

    public abstract class CharacterComponent : HouraiBehaviour {
        
        public Character Character { get; private set; }

        protected virtual void Start() {
            Character = GetComponentInParent<Character>();
            if (Character == null) {
                enabled = false;
                Debug.LogWarning(GetType() + " on " + name +
                                 " has not found a suitable Character component. Please attach one.");
            }
        }

    }

}