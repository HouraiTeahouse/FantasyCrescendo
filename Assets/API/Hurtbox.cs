using System;
using UnityEngine;
using System.Collections.Generic;

namespace Genso.API {

    public enum HitboxType {

        Offensive,
        Damageable,
        Invincible,
        Intangible

    }


    public static class Hurtbox {

        static Hurtbox() {
            HurtboxMap = new Dictionary<Collider, Character>();
            GlobalCallbacks.OnLoad += delegate(int level) {
                                          HurtboxMap.Clear();
                                      };
        }

        private static readonly Dictionary<Collider, Character> HurtboxMap;

        public static void Register(Character character, Collider hurtbox) {
            if(character == null)
                throw new ArgumentNullException("character");
            if(hurtbox == null)
                throw new ArgumentNullException("hurtbox");
            HurtboxMap[hurtbox] = character;
        }

        public static Character GetCharacter(Collider hurtbox) {
            if (hurtbox != null  && HurtboxMap.ContainsKey(hurtbox))
                return HurtboxMap[hurtbox];
            return null;
        }

    }

}
