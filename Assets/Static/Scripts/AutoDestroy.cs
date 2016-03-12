using UnityEngine;

namespace HouraiTeahouse {
    public class AutoDestroy : MonoBehaviour {
        private Animation _animation;
        private AudioSource _audio;
        private ParticleSystem _particleSystem;

        void Awake() {
            _animation = GetComponent<Animation>();
            _audio = GetComponent<AudioSource>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update() {
            if (_animation && _animation.isPlaying)
                return;
            if (_audio && _audio.isPlaying)
                return;
            if (_particleSystem && _particleSystem.isPlaying)
                return;
            Destroy(gameObject);
        }
    }
}
