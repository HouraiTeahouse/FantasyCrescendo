using System.Threading.Tasks;
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

  public static TaskCompletionSource<object> LoadTask = new TaskCompletionSource<object>();

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
#if UNITY_EDITOR
  void Awake() {
    foreach (var type in ValidImportTypes) {
      RegisterAll(EditorAssetUtil.LoadAll(type));
    }
#else
  async void Awake() {
    RegisterAll(GameModes);
    var paths = await GetAllValidPaths(BundleSearch);
    var bundles = paths.Select(async path => {
      var bundle = await AssetBundleManager.LoadAssetBundleAsync(path);
      var asset = await LoadMainAsset(bundle);
      ProcessLoadedAsset(asset, path);
    });
    await Task.WhenAll(bundles);
#endif
    LoadTask.SetResult(new object());
    Debug.Log("Finished loading data");
  }

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

  async Task<IEnumerable<string>> GetAllValidPaths(string[] searchPatterns) {
    var allSets = await Task.WhenAll(searchPatterns.Select(pattern => AssetBundleManager.GetValidBundlePaths(pattern)));
    return allSets.SelectMany(s => s).Distinct();
  }

  async Task<Object> LoadMainAsset(LoadedAssetBundle bundle) {
    var assetBundle = bundle.AssetBundle;
    if (assetBundle.mainAsset != null) {
      return assetBundle.mainAsset;
    } else {
      var mainPath = assetBundle.GetAllAssetNames()[0];
      var request = await assetBundle.LoadAssetAsync<Object>(mainPath).ToTask();
      return request.asset;
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
