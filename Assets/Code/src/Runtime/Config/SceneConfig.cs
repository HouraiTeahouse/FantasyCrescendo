using HouraiTeahouse.Loadables;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Scene = HouraiTeahouse.Loadables.Scene;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Scene Config")]
public class SceneConfig : ScriptableObject {

  [SerializeField, Scene] string _mainMenuScene;
  [SerializeField, Scene] string _matchEndScene;
  [SerializeField, Scene] string[] _additionalStageScenes;

  public IScene MainMenuScene => Scene.Get(_mainMenuScene);
  public IScene MatchEndScene => Scene.Get(_matchEndScene);

  ReadOnlyCollection<IScene> _scenes;
  public ReadOnlyCollection<IScene> AdditionalStageScenes { 
    get  {
      if (_scenes == null) {
        _additionalStageScenes = _additionalStageScenes ?? new string[0];
        _scenes = new ReadOnlyCollection<IScene>(
          _additionalStageScenes.Select(Scene.Get).ToArray()
        );
      }
      return _scenes;
    }
  }

}

}

