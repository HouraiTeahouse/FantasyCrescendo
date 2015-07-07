using UnityEngine;
using System.Collections.Generic;

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
        
        //Cache the old matrix and color for drawing Gizmos
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Color oldColor = Gizmos.color;

        //Set the matrix and color to draw the collider properly
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        
        foreach (var collider in colliders) {
            var boxCollider = collider as BoxCollider;
            var sphereCollider = collider as SphereCollider;
            var meshCollider = collider as MeshCollider;
            if (solid) {
                if (boxCollider != null)
                    Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                if(sphereCollider != null)
                    Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
                if(meshCollider != null)
                    Gizmos.DrawMesh(meshCollider.sharedMesh, Vector3.zero);
            } else {
                if (boxCollider != null)
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                if (sphereCollider != null)
                    Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                if (meshCollider != null)
                    Gizmos.DrawWireMesh(meshCollider.sharedMesh, Vector3.zero);
            }
        }

        //Reset the Gizmos color and matrix to how they used to be
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }

}
