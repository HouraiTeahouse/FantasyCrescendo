namespace HouraiTeahouse.SmashBrew {

    public interface ICharacterState {

        // A Character State has two main components:
        //      Constants - Per character constants for configuration
        //      Variables - Variable properties that change over time.
        //                  Should generally need to be synced across the network.

        // Resets the state's variables
        // Called on any need for reset, this includes Character death.
        void ResetState();

    }

    public interface ICharacterAction {

        void Execute(StateSet states);

    }

}
