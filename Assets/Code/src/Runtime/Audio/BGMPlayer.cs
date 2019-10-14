using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class BGMPlayer : MonoBehaviour {

  public AudioSource AudioSource;
  public SceneData Scene;
  public BGM BGM;
  public bool PlayOnAwake;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    if (Scene != null && Scene.Music.Length > 0) {
      BGM = RandomBGM(Scene.Music);
    }
    if (BGM != null) {
      await LoadingScreen.Await(LoadBGM(BGM));
    }
    if (PlayOnAwake) {
      await PlayBGM(BGM);
    } else {
      Mediator.Global.CreateUnityContext(this).Subscribe<MatchStartEvent>(async evt => {
        await PlayBGM(BGM);
      });
    }
  }

  public async Task LoadBGM(BGM bgm) {
    var clip = await bgm.Clip.LoadAssetAsync<AudioClip>();
    if (AudioSource == null || clip == null) return;
    AudioSource.clip = clip;
  }

  public async Task PlayBGM(BGM bgm) {
    await LoadBGM(bgm);
    if (AudioSource == null) return;
    AudioSource.Play();
  }

  BGM RandomBGM(BGM[] bgmSet) => bgmSet[Mathf.FloorToInt(Random.Range(0, bgmSet.Length))];

}

}
