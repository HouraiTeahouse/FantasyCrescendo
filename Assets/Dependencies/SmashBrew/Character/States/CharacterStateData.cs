using System;
using UnityEngine;
using HouraiTeahouse.SmashBrew.States;

namespace HouraiTeahouse.SmashBrew.Characters {

    public enum SmashAttack {
        None, Charge, Attack
    }

    public enum ImmunityType {
        Normal, Invincible, Intangible, SuperArmor
    }

    [Serializable]
    public class CharacterStateData {
        [Tooltip("Corresponding animation for the state")]
        public AnimationClip AnimationClip;
        [Tooltip("Length of time the state lasts")]
        public float Length;
        [Tooltip("Minimum and maxiumum movement speeds. Interpolated based on input magnitude.")]
        public Range MovementSpeed;
        public StateEntryPolicy EntryPolicy = StateEntryPolicy.Normal;
        public ImmunityType DamageType = ImmunityType.Normal;
        public bool CanTurn = true;
        public bool IgnoreCharacterDirection;
        [NonSerialized]
        public SmashAttack SmashAttack;
    }

}

