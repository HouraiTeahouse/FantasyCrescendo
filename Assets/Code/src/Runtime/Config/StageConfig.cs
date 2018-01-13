using HouraiTeahouse.Loadables;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Scene = HouraiTeahouse.Loadables.Scene;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Stage Config")]
public class StageConfig : ScriptableObject {

  [SerializeField, Scene] string[] _additionalStageScenes;

  ReadOnlyCollection<IScene> _scenes;
  public ReadOnlyCollection<IScene> AdditionalStageScenes { 
    get  {
      if (_scenes == null) {
        _additionalStageScenes = _additionalStageScenes ?? new string[0];
        _scenes = new ReadOnlyCollection<IScene>(
          _additionalStageScenes.Select(Scene.Get).ToArray()
        );
        Debug.LogError(_scenes.Count);
      }
      return _scenes;
    }
  }

}

}

