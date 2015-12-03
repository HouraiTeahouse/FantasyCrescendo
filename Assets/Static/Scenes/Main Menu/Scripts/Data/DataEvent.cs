using Hourai.Events;

/// <summary>
/// Data commands.
/// </summary>
public class DataEvent : IEvent {

    public class ChangePlayerLevelCommand : DataEvent {

        public int newLevel;
        public int playerNum;

    }

    public class ChangePlayerMode : DataEvent {

        public int playerNum;

    }

    /// <summary>
    /// This command says that the users is modifying some options in the menu screen.
    /// </summary>
    public class UserChangingOptions : DataEvent {

        public bool isUserChangingOptions;

    }

    /// <summary>
    /// This command says that the fight is ready to start.
    /// </summary>
    public class ReadyToFight : DataEvent {

        public bool isReady;

    }

}