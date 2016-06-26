using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// AnimationEvent callbacks for SmashBrew Characters
    /// </summary>
    public class CharacterAnimationEvents : CharacterComponent {

        public static string HitboxFunc = "Hitbox";

        public void Hitbox(AnimationEvent animationEvent) {
            var state = animationEvent.intParameter;
            Hitbox hitbox = Character.GetHitbox(Mathf.Abs(state));
            if (hitbox == null)
                Log.Info("Failed to spawn a hitbox with the ID: {0}.", state);
            else
                hitbox.IsActive = state >= 0;
        }

    }

}

