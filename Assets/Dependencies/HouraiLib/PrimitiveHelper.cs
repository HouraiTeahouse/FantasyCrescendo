using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveHelper {

    static readonly Dictionary<PrimitiveType, Mesh> PrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

    public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider) {
        if (withCollider) { return GameObject.CreatePrimitive(type); }

        var gameObject = new GameObject(type.ToString());
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = GetPrimitiveMesh(type);
        gameObject.AddComponent<MeshRenderer>();

        return gameObject;
    }

    public static Mesh GetPrimitiveMesh(PrimitiveType type) {
        if (!PrimitiveMeshes.ContainsKey(type))
            CreatePrimitiveMesh(type);
        return PrimitiveMeshes[type];
    }

    static Mesh CreatePrimitiveMesh(PrimitiveType type) {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(gameObject);

        PrimitiveMeshes[type] = mesh;
        return mesh;
    }
}
