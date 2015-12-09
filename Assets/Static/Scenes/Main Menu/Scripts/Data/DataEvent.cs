using Hourai.Events;
using Hourai.SmashBrew;

/// <summary>
/// Data commands.
/// </summary>
public class DataEvent {

    public class ChangePlayerLevelCommand : DataEvent {

        public int NewLevel;
        public Player Player;

    }

    public class ChangePlayerMode : DataEvent {

        public Player Player;

    }

    /// <summary>
    /// This command says that the users is modifying some options in the menu screen.
    /// </summary>
    public class UserChangingOptions : DataEvent {

        public bool IsUserChangingOptions;

    }

    /// <summary>
    /// This command says that the fight is ready to start.
    /// </summary>
    public class ReadyToFight : DataEvent {

        public bool IsReady;

    }

}