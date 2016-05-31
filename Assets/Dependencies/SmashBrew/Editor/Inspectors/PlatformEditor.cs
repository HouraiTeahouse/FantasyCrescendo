using HouraiTeahouse.Editor;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Editor {

    /// <summary>
    /// A custom Editor for Platfoorms
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof (Platform), true, isFallback = true)]
    internal sealed class StageBasedEditor : ScriptlessEditor {

        /// <summary>
        /// <see cref="ScriptlessEditor.OnInspectorGUI"/>
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Trigger"))
                GenerateTriggerCollider();
        }

        // Collider.bounds gives bounds in world space. We need it in local space
        Bounds GetBounds(Collider collider) {
            var boxCol = collider as BoxCollider;
            var sphereCol = collider as SphereCollider;
            //TODO: Draw Capsule Colliders
            //var capsuleCollider = collider as CapsuleCollider;

            if (boxCol != null)
                return new Bounds(boxCol.center, boxCol.size);
            if (sphereCol != null)
                return new Bounds(sphereCol.center, Vector3.one*sphereCol.radius);
            return new Bounds();
        }

        // Creates or updates the trigger collider
        void GenerateTriggerCollider() {
            var found = false;
            var bounds = new Bounds();

            foreach (Object obj in targets) {
                var platform = obj as Platform;
                if (!platform)
                    continue;
                foreach (Collider collider in platform.GetComponents<Collider>()) {
                    if (collider.isTrigger) continue;
                    if (!found) {
                        found = true;
                        bounds = GetBounds(collider);
                    }
                    else
                        bounds.Encapsulate(GetBounds(collider));
                }

                if (!found)
                    return;

                BoxCollider[] boxColliders = platform.GetComponents<BoxCollider>();
                BoxCollider trigger = null;

                foreach (BoxCollider boxCollider in boxColliders) {
                    if (boxCollider.isTrigger)
                        trigger = boxCollider;
                }

                if (trigger == null)
                    trigger = platform.gameObject.AddComponent<BoxCollider>();

                Vector3 size = bounds.size;
                trigger.center = -0.3f * size.y * Vector3.up + bounds.center;
                size.x *= 1.1f;
                size.y *= 1.3f;
                trigger.size = size;
                trigger.isTrigger = true;
            }
        }

    }

}
