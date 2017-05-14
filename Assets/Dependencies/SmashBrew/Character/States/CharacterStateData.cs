using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public enum SmashAttack {
        None, Charge, Attack
    }
    

    [Serializable]
    public class CharacterStateData {
        public bool IsActive = true;
        [Tooltip("Minimum and maxiumum movement speeds. Interpolated based on input magnitude.")]
        public Range MovementSpeed;
        public bool Invincibility;
        public bool SuperArmor;
        [NonSerialized]
        public SmashAttack SmashAttack;
    }

}

