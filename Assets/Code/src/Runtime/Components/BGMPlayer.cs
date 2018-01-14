using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class BGMPlayer : MonoBehaviour {

  public AudioSource AudioSource;
  public SceneData Scene;
  public BGM BGM;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (Scene != null && Scene.Music.Length > 0) {
      BGM = RandomBGM(Scene.Music);
    }
    if (BGM != null) {
      PlayBGM(BGM);
    }
  }

  public async Task PlayBGM(BGM bgm) {
    if (AudioSource == null) return;
    var clip = await bgm.Clip.LoadAsync();
    AudioSource.clip = clip;
    AudioSource.Play();
  }

  BGM RandomBGM(BGM[] bgmSet) => bgmSet[Mathf.FloorToInt(Random.Range(0, bgmSet.Length))];

}

}
