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
    /// <summary> A Status effect that prevents players from taking damage while active. </summary>
    [DisallowMultipleComponent]
    [Required]
    public sealed class Invincibility : Status {
        PlayerDamage _damage;
        Hitbox[] _hitboxes;

        /// <summary> Unity callback. Called once before the object's first frame. </summary>
        protected override void Start() {
            base.Start();
            _damage = GetComponent<PlayerDamage>();
            _damage.DamageModifiers.In.Add(InvincibilityModifier, int.MaxValue);
            _hitboxes = GetComponentsInChildren<Hitbox>();
        }

        /// <summary> Unity callback. Called on object destruction. </summary>
        void OnDestroy() {
            if (_damage)
                _damage.DamageModifiers.In.Remove(InvincibilityModifier);
        }

        /// <summary> Unity callback. Called when component is enabled. </summary>
        void OnEnable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (Hitbox hitbox in _hitboxes)
                if (hitbox.DefaultType == Hitbox.Type.Damageable)
                    hitbox.DefaultType = Hitbox.Type.Invincible;
        }

        /// <summary> Unity callback. Called when component is disabled. </summary>
        void OnDisable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (Hitbox hitbox in _hitboxes)
                if (hitbox.DefaultType == Hitbox.Type.Invincible)
                    hitbox.DefaultType = Hitbox.Type.Damageable;
        }

        /// <summary> Invincibilty modifier. Negates all damage while active. </summary>
        float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }
    }
}