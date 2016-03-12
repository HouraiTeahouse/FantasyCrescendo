using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// Event fired when a player jumps
    /// </summary>
    public class PlayerJumpEvent {
        // whether the player was on the Ground when jumping or not
        public bool Ground;

        // The number of remaining jumps after this one
        public int RemainingJumps;
    }

    /// <summary>
    /// Event fired when a player touches the Ground or leaves the gum
    /// </summary>
    public class PlayerGroundEvent {
        // whether the player is now on the gorund or not
        public bool Grounded;
    }

    /// <summary>
    /// Event fired when a player dies
    /// </summary> 
    public class PlayerDieEvent {
        // whether or not the player has been revived or not
        public bool Revived;

        // the Player that died
        public Player Player;
    }

    /// <summary>
    /// Event fired when a player first spawns into the match
    /// </summary>
    public class PlayerSpawnEvent {
        // the player that was spawned
        public Player Player;

        // the player's controlled GameObject
        public GameObject PlayerObject;
    }

    /// <summary>
    /// Event fired when a player respawns after dying
    /// </summary>
    public class PlayerRespawnEvent {
        // whether the player has been revived already or not
        public bool Consumed;

        // the player that is to be respawned
        public Player Player;
    }

    /// <summary>
    /// Event fired when a player is damaged
    /// </summary>
    public class PlayerDamageEvent {
        // the damage dealt to the player
        public float Damage;

        // the final damage after being damaged
        public float CurrentDamage;
    }

    /// <summary>
    /// Event fired when a player is healed
    /// </summary>
    public class PlayerHealEvent {
        // the amount of damage that has been healed
        public float Healing;

        // the final damage after healed 
        public float CurrentDamage;
    }

    /// <summary>
    /// Event fired when a player is knocked back
    /// </summary>
    public class PlayerKnockbackEvent {
    }
}