using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> Plays a BGM in response to an event </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayBGM : SingleActionBehaviour {

        static ILog _log = Log.GetLogger("BGM");

        BGMData _currentBGM;

        [SerializeField]
        [Tooltip("The BGM group to sample a BGM from")]
        BGMGroup _group;

        [SerializeField]
        [Tooltip("The audio source to play the music out of")]
        AudioSource _musicSource;

        /// <summary> The currently playing BGM. </summary>
        public BGMData CurrentBGM {
            get { return _currentBGM; }
        }

        /// <summary> Unity callback. Called on object instantiation </summary>
        protected override void Awake() {
            if (!_group) {
                _log.Error("No BGM for {0} specified. No ", name);
                enabled = false;
                return;
            }
            gameObject.GetOrAddComponent<SoundEffect>().hideFlags = HideFlags.HideInInspector;
            if (!_musicSource)
                _musicSource = GetComponent<AudioSource>();
            base.Awake();
        }

        /// <summary> Plays a new BGM from the current BGMGroup and plays it. If already playing, will reselect a new BGM. </summary>
        public void Play() { Play(_group); }

        /// <summary> Plays a random clip from a </summary>
        /// <param name="bgmGroup"> </param>
        public void Play(BGMGroup bgmGroup) {
            if (!bgmGroup) {
                _musicSource.Stop();
                return;
            }
            _group = bgmGroup;
            Play(bgmGroup.GetRandom());
        }

        /// <summary> Plays a BGM </summary>
        /// <param name="bgm"> </param>
        public void Play(BGMData bgm) {
            bgm.BGM.LoadAsync()
                .Then(clip => {
                    if (_currentBGM != null && _currentBGM.BGM.IsLoaded)
                        _currentBGM.BGM.Unload();
                    _musicSource.Stop();
                    _musicSource.clip = clip;
                    _musicSource.Play();
                    _currentBGM = bgm;
                })
                .Done();
        }

        /// <summary> Unity callback. Called repeatedly at a fixed timestep. </summary>
        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (_currentBGM == null || !_musicSource.isPlaying)
                return;
            var loopEnd = _currentBGM.LoopEnd > 0 ? _currentBGM.LoopEnd : _musicSource.clip.samples;
            if (_musicSource.timeSamples >= loopEnd)
                _musicSource.timeSamples = _musicSource.timeSamples + _currentBGM.LoopStart - _currentBGM.LoopEnd;
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() { Play(); }

    }

}
