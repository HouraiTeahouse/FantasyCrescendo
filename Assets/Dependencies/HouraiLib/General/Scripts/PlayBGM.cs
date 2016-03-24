using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// Plays a BGM in response to an event
    /// </summary>
    [RequireComponent(typeof (AudioSource))]
    public class PlayBGM : SingleActionBehaviour {
        [SerializeField, Tooltip("The audio source to play the music out of")] private AudioSource _musicSource;

        [SerializeField, Tooltip("The BGM group to sample a BGM from")] private BGMGroup _group;

        private BGMData _currentBGM;

        /// <summary>
        /// The currently playing BGM.
        /// </summary>
        public BGMData CurrentBGM {
            get { return _currentBGM; }
        }

        /// <summary>
        /// Unity callback. Called on object instantiation  
        /// </summary>
        protected override void Awake() {
            if (!_group) {
                Destroy(this);
                return;
            }
            var effect = gameObject.GetOrAddComponent<SoundEffect>();
            effect.hideFlags = HideFlags.HideInInspector;
            if (!_musicSource)
                _musicSource = GetComponent<AudioSource>();
            base.Awake();
        }

        /// <summary>
        /// Plays a new BGM from the current BGMGroup and plays it.
        /// If already playing, will reselect a new BGM.
        /// </summary>
        public void Play() {
            Play(_group);
        }

        /// <summary>
        /// Plays a random clip from a 
        /// </summary>
        /// <param name="bgmGroup"></param>
        public void Play(BGMGroup bgmGroup) {
            if (!bgmGroup)
                return;
            _group = bgmGroup;
            Play(bgmGroup.GetRandom());
        }

        /// <summary>
        /// Plays a BGM
        /// </summary>
        /// <param name="bgm"></param>
        public void Play(BGMData bgm) {
            _currentBGM = bgm;
            _musicSource.Stop();
            _musicSource.clip = bgm.BGM.Load();
            _musicSource.Play();
        }

        /// <summary>
        /// Unity callback. Called repeatedly at a fixed timestep.
        /// </summary>
        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (_currentBGM == null || !_musicSource.isPlaying)
                return;
            if (_musicSource.timeSamples >= _currentBGM.LoopEnd)
                _musicSource.timeSamples = _musicSource.timeSamples + _currentBGM.LoopStart - _currentBGM.LoopEnd;
        }

        /// <summary>
        /// <see cref="SingleActionBehaviour.Action"/>
        /// </summary>
        protected override void Action() {
            Play();
        }
    }
}
