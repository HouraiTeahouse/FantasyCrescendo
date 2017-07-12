using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters.Statuses {

    /// <summary> A Status effect that prevents players from taking damage while active. </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Characters/Status/Invincibility")]
    public sealed class Invincibility : Status {

        //PlayerDamage _damage;
        //Hitbox[] _hitboxes;

        ///// <summary> Unity callback. Called once before the object's first frame. </summary>
        //void Start() {
        //    _damage = GetComponent<PlayerDamage>();
        //    _damage.DamageModifiers.In.Add(InvincibilityModifier, int.MaxValue);
        //    _hitboxes = GetComponentsInChildren<Hitbox>();
        //}

        ///// <summary> Unity callback. Called on object destruction. </summary>
        //void OnDestroy() {
        //    if (_damage)
        //        _damage.DamageModifiers.In.Remove(InvincibilityModifier);
        //}

        ///// <summary> Unity callback. Called when component is enabled. </summary>
        //void OnEnable() {
        //    if (_hitboxes == null)
        //        _hitboxes = GetComponentsInChildren<Hitbox>();
        //    foreach (Hitbox hitbox in _hitboxes)
        //        if (hitbox.DefaultType == Hitbox.Type.Damageable)
        //            hitbox.DefaultType = Hitbox.Type.Invincible;
        //}

        ///// <summary> Unity callback. Called when component is disabled. </summary>
        //void OnDisable() {
        //    if (_hitboxes == null)
        //        _hitboxes = GetComponentsInChildren<Hitbox>();
        //    foreach (Hitbox hitbox in _hitboxes)
        //        if (hitbox.DefaultType == Hitbox.Type.Invincible)
        //            hitbox.DefaultType = Hitbox.Type.Damageable;
        //}

        ///// <summary> Invincibilty modifier. Negates all damage while active. </summary>
        //float InvincibilityModifier(object source, float damage) { return enabled ? damage : 0f; }

    }

}
