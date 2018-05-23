using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class AudioManager : MonoBehaviour {

  public static AudioManager Instance { get; private set; }

  PrefabPool<ManagedAudio> Pool;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;
    Pool = new PrefabPool<ManagedAudio>(() => {
      var gameObj = new GameObject("AudioSource", typeof(TimeScaledAudio), typeof(ManagedAudio));
      gameObj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
      return gameObj.GetComponent<ManagedAudio>();
    });
  }

  public AudioSource Rent() {
    ManagedAudio audio = Pool.Rent();
    audio.gameObject.SetActive(true);
    return audio.AudioSource;
  }

  internal void Return(ManagedAudio audio) {
    audio.AudioSource.Stop();
    audio.gameObject.SetActive(false);
    Pool.Return(audio);
  }

}

}