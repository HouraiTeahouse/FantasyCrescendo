// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> Events fired when a player jumps </summary>
    public class PlayerJumpEvent {
        // whether the player was on the Ground when jumping or not
        public bool Ground;

        // The number of remaining jumps after this one
        public int RemainingJumps;
    }

    /// <summary> Events fired when a player touches the Ground or leaves the gum </summary>
    public class PlayerGroundEvent {
        // whether the player is now on the gorund or not
        public bool Grounded;
    }

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

    /// <summary> Events fired when a player is damaged </summary>
    public class PlayerDamageEvent {
        // the final damage after being damaged
        public float CurrentDamage;
        // the damage dealt to the player
        public float Damage;
    }

    /// <summary> Events fired when a player is healed </summary>
    public class PlayerHealEvent {
        // the final damage after healed 
        public float CurrentDamage;
        // the amount of damage that has been healed
        public float Healing;
    }

    /// <summary> Events fired when a player is knocked back </summary>
    public class PlayerKnockbackEvent {
    }
}
