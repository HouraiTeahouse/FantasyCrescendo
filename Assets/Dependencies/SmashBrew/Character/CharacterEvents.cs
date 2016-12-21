using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> Events fired when a player dies </summary>
    public class PlayerDieEvent {

        // the Player that died
        public Player Player;
        // whether or not the player has been revived or not
        public bool Revived;

    }

    /// <summary> Events fired when a player first spawns into the match </summary>
    public class PlayerSpawnEvent {

        // the player that was spawned
        public Player Player;

        // the player's controlled GameObject
        public GameObject PlayerObject;

    }

    /// <summary> Events fired when a player respawns after dying </summary>
    public class PlayerRespawnEvent {

        // whether the player has been revived already or not
        public bool Consumed;

        // the player that is to be respawned
        public Player Player;

    }

}
