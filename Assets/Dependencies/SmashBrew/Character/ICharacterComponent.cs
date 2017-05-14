using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public interface ICharacterComponent {

        // A Character State has two main components:
        //      Constants - Per character constants for configuration
        //      Variables - Variable properties that change over time.
        //                  Should generally need to be synced across the network.

        // Resets the state's variables
        // Called on any need for reset, this includes Character death.
        void ResetState();

        void UpdateStateContext(CharacterStateContext context);

    }

    public abstract class CharacterComponent : NetworkBehaviour, ICharacterComponent {

        protected Character Character { get; private set; }

        protected virtual void Awake() {
            var registrar = GetComponentInParent<IRegistrar<ICharacterComponent>>();
            if (registrar != null)
                registrar.Register(this);
            Character = registrar as Character;
        }

        public virtual void ResetState() {
        }

        public virtual void UpdateStateContext(CharacterStateContext context) {
        }

    }

}
