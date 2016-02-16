using UnityEngine;

namespace HouraiTeahouse {

    [RequireComponent(typeof(AudioSource))]
    public class PlayBGM : MonoBehaviour {

        [SerializeField]
        private BGMGroup _group;

        private AudioSource _musicSource;
        private BGMData _currentBGM;

        void Start() {
            if (!_group) {
                Destroy(this);
                return;
            }
            var effect = gameObject.AddComponent<SoundEffect>();
            effect.hideFlags = HideFlags.HideInInspector;
            _musicSource = GetComponent<AudioSource>();
            Play(_group);
        }

        public void Play(BGMGroup bgmGroup) {
            if (!bgmGroup)
                return;
            Play(bgmGroup.GetRandom());
        }

        public void Play(BGMData bgm) {
            _currentBGM = bgm;
            _musicSource.Stop();
            _musicSource.clip = bgm.BGM.Load();
            _musicSource.Play();
        }

        void FixedUpdate() {
            if (_currentBGM == null || !_musicSource.isPlaying)
                return;
            if (_musicSource.timeSamples >= _currentBGM.LoopEnd)
                _musicSource.timeSamples = _musicSource.timeSamples + _currentBGM.LoopStart - _currentBGM.LoopEnd;
        }

    }

}
