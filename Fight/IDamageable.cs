namespace Hourai.SmashBrew {

    public interface IDamageable {

        void Damage(IDamageSource source);

    }

    public interface IDamageSource {

        float BaseDamage { get; }

    }

    public interface IHealable {

        void Heal(IHealingSource source);

    }

    public interface IHealingSource {

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