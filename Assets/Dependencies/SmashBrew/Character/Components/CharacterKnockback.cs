using UnityEngine;
using System;

namespace Hourai.SmashBrew {

    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterKnockback : HouraiBehaviour, IKnockbackable {
        
        private ModifierList<Vector2> _defensiveModifiers;

        public ModifierList<Vector2> DefensiveModifiers => _defensiveModifiers;

        public event Action<Vector2> OnKnockback;

        protected override void Awake() {
            base.Awake();
            _defensiveModifiers = new ModifierList<Vector2>();
        }

        public void Knockback(Vector2 knockback) {
            if (!enabled)
                return;
            //TODO: Reimplement
            //if (_defensiveModifiers.Count > 0)
            //    knockback = _defensiveModifiers.Modifiy(knockback);
            

        }
        

    }

}
