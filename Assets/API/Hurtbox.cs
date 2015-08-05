using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crescendo.API {

    public enum HitboxType {

        Offensive,
        Damageable,
        Invincible,
        Intangible

    }


    public static class Hurtbox {

        private static readonly Dictionary<Collider, Character> HurtboxMap;

        static Hurtbox() {
            HurtboxMap = new Dictionary<Collider, Character>();
            GlobalCallbacks.OnLoad += delegate { HurtboxMap.Clear(); };
        }

        public static void Register(Character character, Collider hurtbox) {
            if (character == null)
                throw new ArgumentNullException("character");
            if (hurtbox == null)
                throw new ArgumentNullException("hurtbox");
            HurtboxMap[hurtbox] = character;
        }

        public static Character GetCharacter(Collider hurtbox) {
            if (hurtbox != null && HurtboxMap.ContainsKey(hurtbox))
                return HurtboxMap[hurtbox];
            return null;
        }

    }

}