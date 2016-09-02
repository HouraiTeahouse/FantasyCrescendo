using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> Represents a single object that can be hit </summary>
    public interface IStrikable {

    }

    /// <summary> Represents a single object that can be hit and can take damage from it </summary>
    public interface IDamageable : IStrikable {

        void Damage(object source, float damage);

    }

    /// <summary> Represents a single objec that can be </summary>
    public interface IHealable : IStrikable {

        void Heal(object source, float healing);

    }

    ///
    public interface IKnockbackable : IStrikable {

        void Knockback(object source, Vector2 knockback);

    }

    public interface IAbsorbable {

        void Absorb(object source);

    }

    public interface IReflectable {

        void Reflect(object source);

    }

}