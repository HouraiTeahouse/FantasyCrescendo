using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse.FantasyCrescendo {

public class AudioManager : MonoBehaviour {

  public static AudioManager Instance { get; private set; }

  [Serializable]
  public struct VolumeOptionBinding {
    public string ControlId;
    public Option Option;
  }

  public AudioMixer MasterMixer;
  public VolumeOptionBinding[] OptionBindings;

  PrefabPool<ManagedAudio> Pool;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;
    CreatePool();
    BindOptions();
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

  void CreatePool() {
    Pool = new PrefabPool<ManagedAudio>(() => {
      var gameObj = new GameObject("AudioSource", typeof(TimeScaledAudio), typeof(ManagedAudio));
      gameObj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
      return gameObj.GetComponent<ManagedAudio>();
    });
  }

  void BindOptions() {
    foreach (var binding in OptionBindings) {
      if (binding.Option == null) continue;
      MasterMixer.SetFloat(binding.ControlId, binding.Option.Get<float>());
      binding.Option.OnValueChanged.AddListener(() => {
        MasterMixer.SetFloat(binding.ControlId, binding.Option.Get<float>());
      });
    }
  }

}

}