namespace Hourai.SmashBrew {

    public interface IDamageable {

        void Damage(object source, float damage);

    }

    public interface IHealable {

        void Heal(object source, float healing);

    }

    public interface IKnockbackable {

        void Knockback(IKnockbacker source);

    }

    public interface IKnockbacker {

        bool FlipDirection { get; }
        float Angle { get; }
        float BaseKnockback { get; }
        float Scaling { get; }

    }

}