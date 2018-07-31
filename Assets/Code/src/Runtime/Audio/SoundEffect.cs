using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary> A </summary>
[CreateAssetMenu]
public class SoundEffect : ScriptableObject {

  /// <summary>
  /// Methods for selecting a clip for a SoundEffect
  /// </summary>
  public enum ClipSelectionMethod {
    /// <summary> Randomly selects a clip </summary>
    Random, 
    /// <summary> Cycles through the set in sequential order. </summary>
    Cycle
  }

  /// <summary> 
  /// All playable clips from this specific SoundEffect. Which clip is selected depends
  /// on <see cref="SelectionMethod"/>.
  /// </summary>
  public AudioClip[] Clips;

  /// <summary> The methodology for which clip is selected from <see cref="Clips"/>. </summary>
  public ClipSelectionMethod SelectionMethod;

  /// <summary> Which AudioMixerGroup to output the audio to. </summary>
  public AudioMixerGroup Output;
  [Range(0, 256)] public int Priority = 128;

  /// <summary> How loud the sound effect will be. </summary>
  [Range(0, 1)] public float Volume = 1f;

  /// <summary> What pitch to play the sound effect at. Note: lower pitches will stretch the length of the effect </summary>
  [Range(-3, 3)] public float Pitch = 1f;

  /// <summary> What pitch to play the sound effect at. Note: lower pitches will stretch the length of the effect </summary>
  [Range(0, 1)] public float SpatialBlend;

  /// <summary>
  /// Applies the properties of the SoundEffect to a AudioSource. 
  /// </summary>
  /// <param name="audioSource"></param>
  public void Apply(AudioSource audioSource) {
    if (audioSource == null) {
      throw new ArgumentNullException(nameof(audioSource));
    }
    audioSource.clip = SelectAudioClip();
    audioSource.priority = Priority;
    audioSource.volume = Volume;
    audioSource.outputAudioMixerGroup = Output;
    var timeScaledAudio = audioSource.GetComponent<TimeScaledAudio>();
    if (timeScaledAudio != null) {
      timeScaledAudio.Pitch = Pitch;
    } else {
      audioSource.pitch = Pitch;
    }
  }

  /// <summary>
  /// Plays the sound effect at a given position.
  /// </summary>
  /// <param name="position"> the position to play the sound effect at </param>
  public void Play(Vector3 position = default(Vector3)) {
    if (AudioManager.Instance == null) return;
    var audioSource = AudioManager.Instance.Rent();
    audioSource.transform.position = position;
    Apply(audioSource);
    audioSource.Play();
  }

  /// <summary>
  /// Plays the sound effect at the position of a Transform.
  /// </summary>
  /// <param name="transform"> the object to play the sound effect at </param>
  public void Play(Transform transform) => Play(transform.position);

  AudioClip SelectAudioClip() {
    if (Clips.Length <= 0) return null;
    switch (SelectionMethod) {
      case ClipSelectionMethod.Random: return Clips[Mathf.FloorToInt(Random.value * Clips.Length)];
      default: return Clips[0];
    }
  }

}

}