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

        void Knockback(IKnockbackSource source, float knockback);

    }

    public interface IKnockbackSource {

        float BaseKnockback { get; }
        float Scaling { get; }

    }

}