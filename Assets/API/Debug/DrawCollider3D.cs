using UnityEngine;
using Genso.API;
using DebugUtil = Genso.API.DebugUtil;

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

    //If set to true, it will draw it solid, visible to all
    [SerializeField]
    private bool solid;

    [SerializeField]
    private bool includeChildren;

    void OnDrawGizmos() {
        Collider[] colliders = includeChildren ? GetComponentsInChildren<Collider>() : GetComponents<Collider>();

        if (colliders == null)
            return;

        GizmoUtil.DrawColliders3D(colliders, color, solid);
    }

}
