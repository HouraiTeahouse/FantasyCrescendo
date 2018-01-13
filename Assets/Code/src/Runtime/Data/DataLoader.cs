using HouraiTeahouse.Tasks;
using HouraiTeahouse.Loadables;
using HouraiTeahouse.Loadables.AssetBundles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A iniitalizer component that loads dynamically loadable data into
/// the global Registry.
/// </summary>
public class DataLoader : MonoBehaviour {

  public static ITask LoadTask = new Task();

  /// <summary>
  /// The supported game mode types.
  /// </summary>
  public GameMode[] GameModes;

  public string[] BundleSearch;

  Type[] ValidImportTypes = new [] {
    typeof(GameMode),
    typeof(SceneData),
    typeof(CharacterData)
  };

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
#if !UNITY_EDITOR
    foreach (var type in ValidImportTypes) {
      RegisterAll(LoadAllInEditor(type));
    }
    LoadTask.Resolve();
#else
    RegisterAll(GameModes);
    GetAllValidPaths(BundleSearch)
      .Then(paths => {
        foreach (var path in paths) {
          AssetBundleManager.LoadAssetBundleAsync(path)
            .Then(LoadMainAsset)
            .Then(obj => ProcessLoadedAsset(obj, path));
        }
        LoadTask.Resolve();
      });
#endif
  }

#if UNITY_EDITOR
  IEnumerable<Object> LoadAllInEditor(Type type) {
    var allGUIDs = AssetDatabase.FindAssets($"t:{type.Name}");
    return allGUIDs.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
      .Select(path => AssetDatabase.LoadAssetAtPath<Object>(path));
  }
#endif

  void RegisterAll(IEnumerable<Object> data) {
    foreach (var datum in data) {
      Register(datum);
    }
  }

  bool Register(Object data) {
    foreach (var type in ValidImportTypes) {
      var dataObj = data as IIdentifiable;
      if (dataObj != null && type.IsInstanceOfType(data)) {
        Registry.Register(type, dataObj);
        Debug.Log($"Registered {type.Name}: {data.name} ({dataObj.Id})");
        return true;
      }
    }
    return false;
  }

  ITask<IEnumerable<string>> GetAllValidPaths(string[] searchPatterns) {
    return Task.All(searchPatterns.Select(pattern => AssetBundleManager.GetValidBundlePaths(pattern)))
      .Then(allSets => allSets.SelectMany(s => s).Distinct());
  }

  ITask<Object> LoadMainAsset(LoadedAssetBundle bundle) {
    var assetBundle = bundle.AssetBundle;
    if (assetBundle.mainAsset != null) {
      return Task.FromResult(assetBundle.mainAsset);
    } else {
      var mainPath = assetBundle.GetAllAssetNames()[0];
      return assetBundle.LoadAssetAsync<Object>(mainPath).ToTask().Then(request => request.asset);
    }
  }

  void ProcessLoadedAsset(Object asset, string path) {
    var identifiable = asset as IIdentifiable;
    if (identifiable == null || !Register(asset)) {
      Resources.UnloadAsset(asset);
      AssetBundleManager.UnloadAssetBundle(path);
    }
  }

}

}
