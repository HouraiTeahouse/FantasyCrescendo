using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace HouraiTeahouse {

    [CreateAssetMenu(fileName = "New BGM Group", menuName = "Hourai Teahouse/BGM Group")]
    public class BGMGroup : ScriptableObject {

        [SerializeField]
        [Tooltip("The name of the BGM group")]
        string _name;

        WeightedRNG<BGMData> _selection;

        [SerializeField]
        BGMData[] backgroundMusicData;

        public ReadOnlyCollection<BGMData> MusicData { get; private set; }

        public bool IsInitialized { get; private set; }

        public string Name {
            get { return _name; }
        }

        protected virtual void OnEnable() {
            IsInitialized = false;
            MusicData = new ReadOnlyCollection<BGMData>(backgroundMusicData);
            _selection = new WeightedRNG<BGMData>();
        }

        protected virtual void OnDisable() {
            Save();
        }

        public void Initialize() {
            if (IsInitialized || backgroundMusicData == null)
                return;
            foreach (BGMData bgmData in backgroundMusicData) {
                bgmData.Initialize(Name);
                _selection[bgmData] = bgmData.Weight;
            }
            IsInitialized = true;
        }

        public void Save() {
            if (!IsInitialized)
                return;
            foreach (BGMData bgmData in backgroundMusicData)
                bgmData.Finish(Name);
        }

        public BGMData GetRandom() {
            if(!IsInitialized)
                Initialize();
            return _selection.Select();
        }

#if UNITY_EDITOR

        public void SetBGMClips(IEnumerable<string> resourcePaths) {
            backgroundMusicData = resourcePaths.Select(path => new BGMData(path, 1f)).ToArray();
        }

#endif
    }

    [Serializable]
    public class BGMData {

        const string Delimiter = "/";
        const string Suffix = "weight";

        [SerializeField]
        [Tooltip("The name of the BGM.")]
        string _name;

        [SerializeField]
        [Tooltip("The artist who created this piece of music")]
        string _artist;

        [SerializeField]
        [Tooltip("The original name of the song.")]
        string _originalName;

        [SerializeField]
        [Tooltip("The origin source of the song")]
        string _originSource;

        [SerializeField]
        [Resource(typeof(AudioClip))]
        string _bgm;

        [SerializeField]
        [Range(0f, 1f)]
        float _baseWeight = 1f;

        [SerializeField]
        [Tooltip("The sample number of the start point the loop.")]
        int _loopStart;

        [SerializeField]
        [Tooltip("The sample number of the end point the loop.")]
        int _loopEnd;

        public BGMData(string path, float weight) {
            _bgm = path;
            _baseWeight = weight;
        }

        public string Name {
            get { return _name; }
        }

        public string Artist {
            get { return _artist; }
        }

        public string OriginalName {
            get { return _originalName; }
        }

        public string OriginSource {
            get { return _originSource; }
        }

        public Resource<AudioClip> BGM { 
            get { return Resource.Get<AudioClip>(_bgm); }
        }

        public float Weight { get; private set; }

        public int LoopStart {
            get { return _loopStart; }
        }

        public int LoopEnd {
            get { return _loopEnd; }
        }

        string GetKey(string stageName) { return string.Format("{0}{1}{2}_{3}", stageName, Delimiter, _bgm, Suffix); }

        public void Initialize(string stageName) {
            string key = GetKey(stageName);
            if (Prefs.Exists(key))
                Weight = Prefs.GetFloat(key);
            else {
                Weight = _baseWeight;
                Prefs.SetFloat(key, Weight);
            }
        }

        public void Finish(string stageName) { Prefs.SetFloat(GetKey(stageName), Weight); }

        public override string ToString() { return string.Format("{0} - ({1})", _bgm, _baseWeight); }

    }

}
