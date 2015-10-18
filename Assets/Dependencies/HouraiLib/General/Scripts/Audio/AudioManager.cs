using UnityEngine;
using UnityEngine.Audio;
using System;

namespace Hourai {
    
    public sealed class AudioManager : Singleton<AudioManager> {

        private static AudioSource bgmSource;

        [SerializeField]
        private AudioMixerGroup musicMixerGroup;

        [SerializeField]
        private AudioMixerGroup sfxMixerGroup;

        public static AudioClip CurrentlyPlaying {
            get { return bgmSource.clip; }
        }

        public static void PlayBGM(AudioClip bgm) {
            bgmSource.Stop();
            bgmSource.clip = bgm;
            bgmSource.Play();
        }

        public static void StopBGM() {
            bgmSource.Stop();
        }

        #region Unity Callbacks
        protected override void Awake() {
            base.Awake();
            TimeBasedAudio tba = gameObject.AddComponent<TimeBasedAudio>();
            tba.DestroyObject = false;
            bgmSource = tba.Audio;
            bgmSource.outputAudioMixerGroup = musicMixerGroup;
            bgmSource.hideFlags = HideFlags.HideInInspector;
            bgmSource.volume = 1f;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f;
        }

        private void OnDestroy() {
            Destroy(bgmSource);
        }
        #endregion
        
        public static AudioSource PlaySFX(AudioClip clip, Vector3 point, float volume = 1f, float pitch = 1f) {
            //Create an empty GameObject
            GameObject go = new GameObject();
            go.transform.position = point;

            TimeBasedAudio tba = go.AddComponent<TimeBasedAudio>();
            AudioSource source = tba.Audio;
            tba.Pitch = pitch;
            source.clip = clip;
            source.volume = volume;
            source.outputAudioMixerGroup = Instance ? Instance.sfxMixerGroup : null;
            source.Play();
            return source;
        }
        
        public static AudioSource PlaySFX(AudioClip clip, GameObject go, float volume = 1f, float pitch = 1f) {
            if (!go)
                throw new ArgumentNullException();

            //Create the source
            TimeBasedAudio tba = go.AddComponent<TimeBasedAudio>();
            AudioSource source = tba.Audio;
            tba.Pitch = pitch;
            tba.DestroyGameObject = true;
            source.clip = clip;
            source.volume = volume;
            source.outputAudioMixerGroup = Instance ? Instance.sfxMixerGroup : null;
            source.Play();
            return source;
        }
    }
}