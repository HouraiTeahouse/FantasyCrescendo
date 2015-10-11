namespace Hourai.SmashBrew {
    
    public class Invincibility : Status {

        protected override void Start() {
            base.Start();
            if (Character.Damage == null)
                Destroy(this);
            else
                Character.Damage.AddDamageModifier(InvincibilityModifier, int.MaxValue);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (Character.Damage)
                Character.Damage.RemoveDamageModifier(InvincibilityModifier);
        }

        float InvincibilityModifier(IDamager source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

