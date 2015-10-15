namespace Hourai.SmashBrew {
    
    public class Invincibility : Status {

        protected override void Start() {
            base.Start();
            if (Character)
                Character.DamageTaken.Add(InvincibilityModifier, int.MaxValue);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if(Character)
                Character.DamageTaken.Remove(InvincibilityModifier);
        }

        float InvincibilityModifier(IDamager source, float damage) {
            return enabled ? damage : 0f;
        }

    }

}

