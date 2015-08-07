using UnityEngine;
using UnityEditor;

namespace Crescendo.API.Editor {

    [CustomEditor(typeof(TriggerStageElement), true, isFallback = true)]
    public class StageBasedEditor : UnityEditor.Editor {
        
        private Platform _platform;

        void OnEnable()
        {
            _platform = target as Platform;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Trigger"))
                GenerateTriggerCollider();
        }

        Bounds GetBounds(Collider collider)
        {
            var boxCol = collider as BoxCollider;
            var sphereCol = collider as SphereCollider;
            var capsuleCollider = collider as CapsuleCollider;

            if (boxCol != null)
                return new Bounds(boxCol.center, boxCol.size);
            if (sphereCol != null)
                return new Bounds(sphereCol.center, Vector3.one * sphereCol.radius);
            return new Bounds();
        }

        void GenerateTriggerCollider()
        {

            bool found = false;
            Bounds bounds = new Bounds();

            foreach (var collider in _platform.GetComponents<Collider>())
            {

                if (collider != null && !collider.isTrigger)
                {
                    if (!found)
                    {
                        found = true;
                        bounds = GetBounds(collider);
                    }
                    else
                    {
                        bounds.Encapsulate(GetBounds(collider));
                    }
                }
            }

            if (!found)
                return;

            BoxCollider[] boxColliders = _platform.GetComponents<BoxCollider>();
            BoxCollider trigger = null;

            foreach (var boxCollider in boxColliders)
                if (boxCollider.isTrigger)
                    trigger = boxCollider;

            if (trigger == null)
                trigger = _platform.gameObject.AddComponent<BoxCollider>();

            Vector3 size = bounds.size;
            trigger.center = -0.3f * size.y * Vector3.up + bounds.center;
            size.x *= 1.1f;
            size.y *= 1.3f;
            trigger.size = size;
            trigger.isTrigger = true;
        }

    }


}