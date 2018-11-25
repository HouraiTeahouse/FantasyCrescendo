using HouraiTeahouse.EditorAttributes;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Scene Config")]
public class SceneConfig : ScriptableObject {

  //TODO(james7132): Assign type restrictions on these.
  public AssetReference MainMenuScene;
  public AssetReference MatchEndScene;
  public AssetReference ErrorScene;
  public AssetReference[] AdditionalStageScenes;
  [Tag] public string SpawnTag;

}

}

