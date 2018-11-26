using HouraiTeahouse.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary> 
/// Singleton manager of all audio related functions in the game. 
///</summary>
public class AudioManager : MonoBehaviour {

  /// <summary> 
  /// Gets the singleton instance of the AudioManager. 
  /// </summary>
  public static AudioManager Instance { get; private set; }

  /// <summary> 
  /// A serializable binding between an Option and a AudioMixer control. 
  /// </summary>
  [Serializable]
  public struct VolumeOptionBinding {
    /// <summary> 
    /// The control ID. This is the same string that is used as a exposed parameter.
    /// </summary>
    public string ControlId;
    /// <summary> 
    /// The Option bound to the volume control 
    /// </summary>
    public Option Option;
  }

  /// <summary> 
  /// The AudioMixer for the entire game. 
  /// </summary>
  public AudioMixer MasterMixer;

  [Range(-80f, 20f)] public float MaxVolume = 0f;
  [Range(-80f, 20f)] public float MinVolume = -80f;

  /// <summary> 
  /// The set of all bindings between AudioMixer volume controls and Options. 
  /// </summary>
  public VolumeOptionBinding[] OptionBindings;

  PrefabPool<ManagedAudio> Pool;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    Instance = this;
    CreatePool();
  }

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start() {
    // This is in Start so that the AudioMixer set can initialize itself before loading the volumes
    BindOptions();
  }

  /// <summary> 
  /// Rents an AudioSource from the sound effects pool. 
  /// </summary>
  /// <remarks>
  /// There is no need to return the AudioSource to the pool. It will automatically be cleaned up
  /// when the sound effect is no longer playing or is destroyed.
  /// </remarks>
  /// <returns> the rented AudioSource </returns>
  public AudioSource Rent() {
    ManagedAudio audio = Pool.Rent();
    audio.gameObject.SetActive(true);
    return audio.AudioSource;
  }

  // Returns an AudioSource back to the pool
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

  // Binds the Options described in OptionBindings to automatically change the volume set in the mixer
  void BindOptions() {
    foreach (var binding in OptionBindings) {
      if (binding.Option == null) continue;
      SetVolume(binding.ControlId, binding.Option.Get<float>());
      binding.Option.OnValueChanged.AddListener(() => {
        SetVolume(binding.ControlId, binding.Option.Get<float>());
      });
    }
  }

  void SetVolume(string controlId, float optionValue) {
    float sqrt = Mathf.Sqrt(optionValue);
    float dbValue = Mathf.Lerp(MinVolume, MaxVolume, sqrt);
    MasterMixer.SetFloat(controlId, dbValue);
  }

}

}