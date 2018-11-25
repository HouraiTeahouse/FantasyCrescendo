using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Fantasy Crescendo/Scene (Stage)")]
public class SceneData : GameDataBase {

  public SceneType Type = SceneType.Stage;
  public string Name;

  public AssetReference Scene;
  [AssetReferenceTypeRestriction(typeof(Sprite))] public AssetReference Icon;
  [AssetReferenceTypeRestriction(typeof(GameObject))] public AssetReference PreviewImage;

  public int LoadPriority;

  public BGM[] Music;

  public override string ToString() => $"Scene ({name})";
}

public enum SceneType {
  Menu, Stage
}

}