using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public interface IDamageable {

        void Damage(object source, float damage);

    }

    public interface IHealable {

        void Heal(object source, float healing);

    }

    public interface IKnockbackable {

        void Knockback(Vector2 knockback);

    }

}
