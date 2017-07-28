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

    public abstract class CharacterNetworkComponent : NetworkBehaviour, ICharacterComponent, IResettable {

        protected Character Character { get; private set; }
        protected CharacterState CurrentState {
            get { return (Character != null && Character.StateController != null) ? Character.StateController.CurrentState : null; }
        }

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

        void IResettable.OnReset() {
            ResetState();
        }

    }

    public abstract class CharacterComponent : BaseBehaviour, ICharacterComponent, IResettable {

        protected Character Character { get; private set; }
        protected CharacterState CurrentState {
            get { return (Character != null && Character.StateController != null) ? Character.StateController.CurrentState : null; }
        }

        protected override void Awake() {
            base.Awake();
            var registrar = GetComponentInParent<IRegistrar<ICharacterComponent>>();
            if (registrar != null)
                registrar.Register(this);
            Character = registrar as Character;
        }

        public virtual void ResetState() {
        }

        public virtual void UpdateStateContext(CharacterStateContext context) {
        }

        void IResettable.OnReset() {
            ResetState();
        }

    }

}
