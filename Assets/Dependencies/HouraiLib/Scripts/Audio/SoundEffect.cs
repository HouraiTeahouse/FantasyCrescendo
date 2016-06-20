// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse {
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundEffect : HouraiBehaviour {
        AudioSource _audio;

        bool destroyOnFinish;

        public AudioSource Audio {
            get { return _audio; }
        }

        public float Pitch { get; set; }

        protected override void Awake() {
            base.Awake();
            _audio = GetComponent<AudioSource>();
            Pitch = _audio.pitch;
        }

        void Update() {
            _audio.pitch = EffectiveTimeScale * Pitch;
            if (destroyOnFinish && !_audio.isPlaying)
                Destroy(gameObject);
        }

        public AudioSource Play() { return Play(Vector3.zero); }

        public AudioSource Play(float volume) {
            AudioSource audioSource = Play();
            audioSource.volume = volume;
            return audioSource;
        }

        public AudioSource Play(Vector3 position) {
            var soundEffect =
                Instantiate(this, position, Quaternion.identity) as SoundEffect;
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