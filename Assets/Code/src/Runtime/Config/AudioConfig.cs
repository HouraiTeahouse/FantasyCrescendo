using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse.FantasyCrescendo {

[CreateAssetMenu(menuName = "Config/Audio Config")]
public class AudioConfig : ScriptableObject {

  public AudioMixerGroup MusicMixer;
  public AudioMixerGroup SoundEffectsMixer;

  public AudioSource CreateSFXAudioSource(float volume = 1.0f) {
    var source = new GameObject("SFX Audio Source").AddComponent<AudioSource>();
    source.outputAudioMixerGroup = SoundEffectsMixer;
    source.volume = volume;
    return source;
  }

}

}
