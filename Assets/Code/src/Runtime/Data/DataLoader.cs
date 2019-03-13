using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A iniitalizer component that loads dynamically loadable data into
/// the global Registry.
/// </summary>
public class DataLoader : MonoBehaviour {

  public static TaskCompletionSource<object> LoadTask = new TaskCompletionSource<object>();

  /// <summary>
  /// The supported game mode types.
  /// </summary>
  public GameMode[] GameModes;

  public AssetLabelReference[] LoadedLabels;

  static Type[] ValidImportTypes = new [] {
    typeof(GameMode),
    typeof(SceneData),
    typeof(CharacterData)
  };

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    LoadingScreen.Await(LoadTask.Task);
    RegisterAll(GameModes);
    await Task.WhenAll(LoadedLabels.Select(async label => {
      await Addressables.LoadAssets<UnityEngine.Object>(label.labelString, async infoOp => {
        var info = await infoOp;
        if (info != null && !Register(info)) {
          Addressables.ReleaseAsset(info);
        }
      });
    }));
    LoadTask.TrySetResult(new object());
    Debug.Log("Finished loading data");
  }

  void RegisterAll(IEnumerable<UnityEngine.Object> data) {
    foreach (var datum in data) {
      Register(datum);
    }
  }

  bool Register(UnityEngine.Object data) {
    var dataObj = data as IEntity;
    if (dataObj == null) {
      return false;
    }
    foreach (var type in ValidImportTypes) {
      if (type.IsInstanceOfType(data)) {
        Registry.Register(type, dataObj);
        Debug.Log($"Registered {type.Name}: {data.name} ({dataObj.Id})");
        return true;
      }
    }
    return false;
  }

}

}
