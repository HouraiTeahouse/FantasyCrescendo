/// <summary>
/// Data commands.
/// </summary>
public class DataCommands {

    public class ChangePlayerLevelCommand : Command {

        public int newLevel;
        public int playerNum;

    }

    public class ChangePlayerMode : Command {

        public int playerNum;

    }

    /// <summary>
    /// This command says that the users is modifying some options in the menu screen.
    /// </summary>
    public class UserChangingOptions : Command {

        public bool isUserChangingOptions;

    }

    /// <summary>
    /// This command says that the fight is ready to start.
    /// </summary>
    public class ReadyToFight : Command {

        public bool isReady;

    }

}