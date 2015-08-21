namespace Hourai.SmashBrew {
    
    public class Invincibility : Status {

        protected override void Start() {
            base.Start();
            if (Character.Damage == null)
                Destroy(this);
            else
                Character.Damage.AddDamageModifier(InvincibilityModifier, int.MaxValue);
        }

        void OnDestroy() {
            if (Character.Damage)
                Character.Damage.RemoveDamageModifier(InvincibilityModifier);
        }

        float InvincibilityModifier(IDamageSource source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

