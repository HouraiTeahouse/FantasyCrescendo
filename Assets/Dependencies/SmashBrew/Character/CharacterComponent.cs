using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public abstract class CharacterComponent : HouraiBehaviour {
        
        public Character Character { get; private set; }

        /// <summary>
        /// The events mediator for the subscribed character.
        /// Do not access before Start is called.
        /// </summary>
        protected Mediator CharacterEvents { get; private set; }

        protected virtual void Start() {
            Character = GetComponentInParent<Character>();
            if (Character == null) {
                enabled = false;
                Debug.LogWarning(GetType() + " on " + name +
                                 " has not found a suitable Character component. Please attach one.");
            } else {
                CharacterEvents = Character.CharacterEvents;
            }
        }

    }

}
