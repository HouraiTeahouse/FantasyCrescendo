using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Object = UnityEngine.Object;

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
            var bgmEffect = gameObject.AddComponent<SoundEffect>();
            bgmSource = bgmEffect.Audio;
            bgmSource.outputAudioMixerGroup = musicMixerGroup;
            bgmSource.hideFlags = HideFlags.HideInInspector;
            bgmSource.volume = 1f;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f;
        }

        private static IEnumerator DestroyOnFinish(AudioSource audio, params Object[] targets) {
            while (audio.isPlaying)
                yield return null;
            foreach(var obj in targets)
                Destroy(obj);
        }

        private void OnDestroy() {
            Destroy(bgmSource);
        }
        #endregion
        
        public static SoundEffect PlaySFX(AudioClip clip, Vector3 point, float volume = 1f, float pitch = 1f) {
            //Create an empty GameObject
            var go = new GameObject();
            go.transform.position = point;

            var soundEffect = go.AddComponent<SoundEffect>();
            AudioSource source = soundEffect.Audio;
            soundEffect.Pitch = pitch;
            source.clip = clip;
            source.volume = volume;
            source.outputAudioMixerGroup = Instance ? Instance.sfxMixerGroup : null;
            source.Play();
            soundEffect.StartCoroutine(DestroyOnFinish(source, go));
            return soundEffect;
        }
        
        public static SoundEffect PlaySFX(AudioClip clip, GameObject go, float volume = 1f, float pitch = 1f) {
            if (!go)
                throw new ArgumentNullException();

            //Create the source
            var soundEffect = go.AddComponent<SoundEffect>();
            AudioSource source = soundEffect.Audio;
            soundEffect.Pitch = pitch;
            source.clip = clip;
            source.volume = volume;
            source.outputAudioMixerGroup = Instance ? Instance.sfxMixerGroup : null;
            source.Play();
            soundEffect.StartCoroutine(DestroyOnFinish(source, source, soundEffect));
            return soundEffect;
        }
    }
}