using UnityEngine;

namespace Hourai {
    
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundEffect : HouraiBehaviour {

        private AudioSource _audio;
        
        public AudioSource Audio {
            get { return _audio; }
        }

        private bool destroyOnFinish;

        public float Pitch { get; set; }

        protected override void Awake() {
            base.Awake();
            _audio = GetComponent<AudioSource>();
            _audio.hideFlags = HideFlags.HideInInspector;
            hideFlags = HideFlags.HideInInspector;
            Pitch = _audio.pitch;
        }

        void Update() {
            _audio.pitch = EffectiveTimeScale * Pitch;
            if(destroyOnFinish && !_audio.isPlaying)
                Destroy(gameObject); 
        }

        public AudioSource Play() {
            return Play(Vector3.zero);
        }

        public AudioSource Play(float volume) {
            AudioSource audioSource = Play();
            audioSource.volume = volume;
            return audioSource;
        }

        public AudioSource Play(Vector3 position) {
            var soundEffect = Instantiate(this, position, Quaternion.identity) as SoundEffect;
            soundEffect.destroyOnFinish = true;
            return soundEffect.Audio;
        }

        public AudioSource Play(float volume, Vector3 position) {
            AudioSource audioSource = Play(position);
            audioSource.volume = volume;
            return audioSource;
        }

    }


}

