using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        #if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            Object.DestroyImmediate(gameObject);
        else
            Object.Destroy(gameObject);
        #else
        Object.Destroy(gameObject);
        #endif

        PrimitiveMeshes[type] = mesh;
        return mesh;
    }
}
