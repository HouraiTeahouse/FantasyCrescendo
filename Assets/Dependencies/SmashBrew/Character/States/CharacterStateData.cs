using HouraiTeahouse.SmashBrew.States;
using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace HouraiTeahouse.SmashBrew.Characters {

    public enum SmashAttack {
        None, Charge, Attack
    }

    public enum ImmunityType {
        Normal, Invincible, Intangible, SuperArmor
    }

    public enum MovementType {
        Normal, DirectionalInfluenceOnly, Locked
    }

    [Serializable]
    public class CharacterStateData {
        [Tooltip("Corresponding timeline controller")]
        public TimelineAsset Timeline;
        [Tooltip("Corresponding animation for the state")]
        public AnimationClip AnimationClip;
        [Tooltip("Length of time the state lasts")]
        public float Length;
        [Tooltip("Minimum and maxiumum movement speeds. Interpolated based on input magnitude.")]
        public Range MovementSpeed;
        public StateEntryPolicy EntryPolicy = StateEntryPolicy.Normal;
        public ImmunityType DamageType = ImmunityType.Normal;
        public MovementType MovementType = MovementType.Normal;
        public bool CanTurn = true;
        public float KnockbackResistance;
        [NonSerialized]
        public SmashAttack SmashAttack;
    }

}

