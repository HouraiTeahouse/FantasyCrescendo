using HouraiTeahouse.FantasyCrescendo;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

public abstract class GameObjectTest {

  List<GameObject> gameObjects = new List<GameObject>();

  [TearDown]
  protected virtual void TearDown() {
    foreach (var go in gameObjects) {
      Object.DestroyImmediate(go);
    }
  }

  protected T CreateObject<T>() where T : Component {
    var gameObject = new GameObject(typeof(T).Name + $" {gameObjects.Count}");
    gameObjects.Add(gameObject);
    return gameObject.AddComponent<T>();
  }

}
