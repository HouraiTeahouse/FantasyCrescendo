using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// Draws Colliders as Gizmos, permanentally seen in the Scene view.
    /// Good for general establishing of boundaries.
    /// Currently does not support CapsuleColliders
    /// </summary>
    public class DrawCollider3D : MonoBehaviour
    {
        [SerializeField, Tooltip("The color used to draw the colliders with.")]
        private Color color;

        [SerializeField, Tooltip("Whether or not to include the Colliders in the children of the GameObject or not.")]
        private bool includeChildren;

        [SerializeField, Tooltip("If set to true, colliders are drawn as solids, otherwise drawn as wireframes.")]
        private bool solid;

#if UNITY_EDITOR
        /// <summary>
        /// Unity Callback. Called in the Editor to draw Gizmos on each GUI update.
        /// </summary>
        void OnDrawGizmos()
        {
            Collider[] colliders = includeChildren ? GetComponentsInChildren<Collider>() : GetComponents<Collider>();

            if (colliders == null)
                return;

            GizmoUtil.DrawColliders3D(colliders, color, solid);
        }
#endif
    }

}
