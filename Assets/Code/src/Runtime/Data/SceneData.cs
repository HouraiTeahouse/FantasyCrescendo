using HouraiTeahouse.Loadables;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/Scene (Stage)")]
public class SceneData : GameDataBase {

  public bool IsStage = true;

  public string Name;

  [SerializeField, Scene] string _scene;

  public IScene GameScene => Scene.Get(_scene);

  public BGM[] Music;
}

}