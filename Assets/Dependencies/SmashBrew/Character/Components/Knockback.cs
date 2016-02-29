using UnityEngine;
using System;
using HouraiTeahouse.SmashBrew.Util;

namespace HouraiTeahouse.SmashBrew {


    /// <summary>
    /// A MonoBehaviour that handles the knockback dealt to a Player
    /// </summary>
    public partial class Character {
        
        private ModifierList<Vector2> _defensiveModifiers;

        public ModifierGroup<Vector2> KnockbackModifiers { get; private set; }

        public event Action<Vector2> OnKnockback;

        /// <summary>
        /// <see cref="IKnockbackable.Knockback"/>
        /// </summary>
        public void Knockback(Vector2 knockback) {
            //TODO: Reimplement
            //if (_defensiveModifiers.Count > 0)
            //    knockback = _defensiveModifiers.Modifiy(knockback);
        }

    }

}
