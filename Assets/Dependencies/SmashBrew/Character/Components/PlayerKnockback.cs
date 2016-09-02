using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A MonoBehaviour that handles the knockback dealt to a Player </summary>
    public sealed class PlayerKnockback : CharacterComponent, IKnockbackable {

        ModifierList<Vector2> _defensiveModifiers;

        public ModifierGroup<Vector2> KnockbackModifiers { get; private set; }

        /// <summary>
        ///     <see cref="IKnockbackable.Knockback" />
        /// </summary>
        public void Knockback(object src, Vector2 knockback) {
            //TODO: Reimplement
            //if (_defensiveModifiers.Count > 0)
            //    knockback = _defensiveModifiers.Modifiy(knockback);
        }

    }

}