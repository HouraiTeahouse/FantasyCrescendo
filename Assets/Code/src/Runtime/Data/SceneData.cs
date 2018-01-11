using HouraiTeahouse.Loadables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/Scene (Stage)")]
public class SceneData : ScriptableObject {
  public string Name;

  public bool IsStage = true;
  public bool IsSelectable = true;
  public bool IsVisible = true;
  public bool IsDebug;

  [SerializeField, Scene] string _scene;

  public IScene GameScene => Scene.Get(_scene);

  public BGM[] Music;
}

}