using System.Collections;
using System.Collections.Generic;
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

  public void PlayBGM(BGM bgm) {
    bgm.Clip.LoadAsync().Then(clip => {
      if (AudioSource != null) {
        AudioSource.clip = clip;
        AudioSource.Play();
      }
    });
  }

  BGM RandomBGM(BGM[] bgmSet) => bgmSet[Mathf.FloorToInt(Random.Range(0, bgmSet.Length))];

}

}
