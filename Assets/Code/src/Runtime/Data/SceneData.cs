using HouraiTeahouse.Loadables;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/Scene (Stage)")]
public class SceneData : GameDataBase {

  public SceneType Type = SceneType.Stage;
  public string Name;

  [SerializeField, Scene] string _scene;
  [SerializeField, Resource(typeof(Sprite))] string _icon;
  [SerializeField, Resource(typeof(Sprite))] string _previewImage;

  public int LoadPriority;

  public BGM[] Music;

  public IScene GameScene => Scene.Get(_scene);
  public IAsset<Sprite> Icon => Asset.Get<Sprite>(_icon);
  public IAsset<Sprite> PreviewImage => Asset.Get<Sprite>(_previewImage);

  public override string ToString() => $"Scene ({name})";
}

public enum SceneType {
  Menu, Stage
}

}