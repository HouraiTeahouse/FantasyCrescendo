using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

public class SceneLoader : MonoBehaviour {

  public bool LoadOnAwake = true;
  public LoadSceneMode Mode;
  public AssetReference[] Scenes;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    if (LoadOnAwake) {
      await LoadScenes();
    }
  }

  public async Task LoadScenes() {
    await Task.WhenAll(Scenes.Select(async s => await Addressables.LoadScene(s, Mode)));
    Debug.Log("Scenes loaded!");
  }

}

}
