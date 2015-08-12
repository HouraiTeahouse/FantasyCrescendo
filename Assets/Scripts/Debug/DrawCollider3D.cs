using Hourai;
using UnityEngine;

/// <summary>
/// Draws Colliders as Gizmos, permanentally seen in the Scene view.
/// Good for general establishing of boundaries.
/// Currently does not support CapsuleColliders
/// </summary>
/// Author: James Liu
/// Authored on 07/01/2015
public class DrawCollider3D : TestScript {

    [SerializeField]
    private Color color;

    [SerializeField]
    private bool includeChildren;

    //If set to true, it will draw it solid, visible to all
    [SerializeField]
    private bool solid;

    private void OnDrawGizmos() {
        Collider[] colliders = includeChildren ? GetComponentsInChildren<Collider>() : GetComponents<Collider>();

        if (colliders == null)
            return;

        GizmoUtil.DrawColliders3D(colliders, color, solid);
    }

}