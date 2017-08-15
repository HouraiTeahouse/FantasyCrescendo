using HouraiTeahouse.SmashBrew;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse {

    [ExecuteInEditMode]
    public class HitboxDisplay : MonoBehaviour {

        [SerializeField]
        KeyCode _key = KeyCode.F11;

        /// <summary> Unity callback. Called once per frame. </summary>
        void Update() {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return;
            #endif
            if (!Debug.isDebugBuild)
                enabled = false;
            if (Input.GetKeyDown(_key))
                Hitbox.DrawHitboxes = !Hitbox.DrawHitboxes;
        }
        
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            #if UNITY_EDITOR
            if (Hitbox.DrawHitboxes || !EditorApplication.isPlayingOrWillChangePlaymode) {
                foreach (var hitbox in FindObjectsOfType<Hitbox>())
                    if (hitbox.isActiveAndEnabled)
                        hitbox.DrawHitbox();
            }
            #else
            if (Hitbox.DrawHitboxes) {
                foreach (var hitbox in Hitbox.ActiveHitboxes)
                    if (hitbox.isActiveAndEnabled)
                        hitbox.DrawHitbox();
            }
            #endif
            Graphics.Blit(src, dst);
        }

    } 

}

