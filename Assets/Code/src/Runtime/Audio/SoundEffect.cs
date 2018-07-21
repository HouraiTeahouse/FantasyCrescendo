using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu]
public class SoundEffect : ScriptableObject {

  public enum ClipSelectionMethod {
    Random, Cycle
  }

  public AudioClip[] Clips;
  public ClipSelectionMethod SelectionMethod;
  public AudioMixerGroup Output;
  [Range(0, 256)] public int Priority = 128;
  [Range(0, 1)] public float Volume = 1f;
  [Range(-3, 3)] public float Pitch = 1f;
  [Range(0, 1)] public float SpatialBlend;

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

  public void Play(Vector3 position = default(Vector3)) {
    if (AudioManager.Instance == null) return;
    var audioSource = AudioManager.Instance.Rent();
    audioSource.transform.position = position;
    Apply(audioSource);
    audioSource.Play();
  }

  public void Play(Transform transform) => Play(transform.position);

  AudioClip SelectAudioClip() {
    switch (SelectionMethod) {
      case ClipSelectionMethod.Random: return Clips[Mathf.FloorToInt(Random.value * Clips.Length)];
      default: return Clips[0];
    }
  }

}

}