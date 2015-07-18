using UnityEngine;
using System.Collections;

namespace Genso.API {

    public sealed class BGM : Singleton<BGM> {

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
            bgmSource = gameObject.SafeGetComponent<AudioSource>();
        }

        void OnDestroy() {
            Destroy(bgmSource);
        }

    }

}
