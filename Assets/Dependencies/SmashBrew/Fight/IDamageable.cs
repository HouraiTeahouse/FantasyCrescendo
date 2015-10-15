namespace Hourai.SmashBrew {

    public interface IDamageable {

        void Damage(IDamager source);

    }

    public interface IDamager {

        float BaseDamage { get; }

    }

    public interface IHealable {

        void Heal(IHealer source);

    }

    public interface IHealer {

        float BaseHealing { get; }

    }

    public interface IKnockbackable {

        void Knockback(IKnockbackSource source);

    }

    public interface IKnockbackSource {

        bool FlipDirection { get; }
        float Angle { get; }
        float BaseKnockback { get; }
        float Scaling { get; }

    }

}