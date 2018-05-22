using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu]
public class SoundEffect : ScriptableObject {

  public AudioClip Clip;
  public AudioMixerGroup Output;
  [Range(0, 256)] public int Priority = 128;
  [Range(0, 1)] public float Volume = 1f;
  [Range(-3, 3)] public float Pitch = 1f;
  [Range(0, 1)] public float SpatialBlend;

  public void Apply(AudioSource audioSource) {
    if (audioSource == null) {
      throw new ArgumentNullException(nameof(audioSource));
    }
    audioSource.clip = Clip;
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
    var audioSource = AudioManager.Instance.Rent();
    audioSource.transform.position = position;
    Apply(audioSource);
    audioSource.Play();
  }

  public void Play(Transform transform) => Play(transform.position);

}

}