using UnityEngine;
using System.Collections;
using System;

namespace Hourai.SmashBrew {

    [RequiredCharacterComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterKnockback : MonoBehaviour, IKnockbackable {
        
        private ModifierList<Vector2> _defensiveModifiers;

        public ModifierList<Vector2> DefensiveModifiers {
            get { return _defensiveModifiers; }
        }

        public event Action<Vector2> OnKnockback;

        private Rigidbody _rigidbody;

        void Start() {
            _rigidbody = GetComponent<Rigidbody>();
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
