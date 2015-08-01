using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Crescendo.API {

    public sealed class BGM : Singleton<BGM> {

		[SerializeField]
		private AudioMixerGroup mixerGroup;

        private static AudioSource bgmSource;

        public static AudioClip CurrentlyPlaying {
            get { return bgmSource.clip; }
        }

        public static void Play(AudioClip bgm) {
            bgmSource.Stop();
            bgmSource.clip = bgm;
            bgmSource.Play();
        }

        public static void Stop() {
            bgmSource.Stop();
        }

        protected override void Awake() {
            base.Awake();
            bgmSource = gameObject.GetOrAddComponent<AudioSource>();
			bgmSource.outputAudioMixerGroup = mixerGroup;
			//bgmSource.hideFlags = HideFlags.HideInInspector;
			bgmSource.volume = 1f;
			bgmSource.spatialBlend = 0f;
        }

        void OnDestroy() {
            Destroy(bgmSource);
        }

    }

}
