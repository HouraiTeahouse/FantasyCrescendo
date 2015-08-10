using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public abstract class UnityUnitTest {

    public GameObject CreateGameObject() {
        return CreateGameObject("");
    }

    public GameObject CreateGameObject(string name) {
        GameObject go = string.IsNullOrEmpty(name) ? new GameObject() : new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "");
        return go;
    }

    public GameObject CreatePrimitive(PrimitiveType type) {
        GameObject p = GameObject.CreatePrimitive(type);
        Undo.RegisterCreatedObjectUndo(p, "");
        return p;
    }

}