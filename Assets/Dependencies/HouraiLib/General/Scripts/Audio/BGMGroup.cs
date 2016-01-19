using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace Hourai {

    public class BGMGroup : ScriptableObject {

        [SerializeField, Tooltip("The name of the BGM group")]
        private string _name;

        [SerializeField]
        private BGMData[] backgroundMusicData;

        private WeightedRNG<BGMData> _selection;

        public string Name {
            get { return _name; }
        }

        protected virtual void OnEnable() {
            _selection = new WeightedRNG<BGMData>();
            if (backgroundMusicData == null)
                return;
            foreach (BGMData bgmData in backgroundMusicData) {
                bgmData.Initialize(Name);
                _selection[bgmData] = bgmData.Weight;
            }
        }

        public BGMData GetRandom() {
            return _selection.Select();
        }

#if UNITY_EDITOR

        public void SetBGMClips(IEnumerable<string> resourcePaths) {
            backgroundMusicData = resourcePaths.Select(path => new BGMData(path, 1f)).ToArray();
        }

#endif 

    }

    [System.Serializable]
    public class BGMData {

        private const string delimiter = "/";
        private const string suffix = "weight";

        [SerializeField]
        [Tooltip("The name of the BGM.")]
        private string _name;

        [SerializeField]
        [Tooltip("The artist who created this piece of music")]
        private string _artist;

        [SerializeField, Resource(typeof(AudioClip))]
        private string _bgm;

        [SerializeField, Range(0f, 1f)]
        private float _baseWeight = 1f;

        [SerializeField]
        [Tooltip("The sample number of the start point the loop.")]
        private int _loopStart;

        [SerializeField]
        [Tooltip("The sample number of the end point the loop.")]
        private int _loopEnd;

        private Resource<AudioClip> _bgmResource;
        private float _weight;
        private string playerPrefsKey;

        public Resource<AudioClip> BGM {
            get { return _bgmResource; }
        }

        public BGMData(string path, float weight) {
            _bgm = path;
            _baseWeight = weight;
        }

        public float Weight {
            get { return _weight; }
        }

        public int LoopStart {
            get { return _loopStart; }
        }

        public int LoopEnd {
            get { return _loopEnd; }
        }

        public void Initialize(string stageName) {
            _bgmResource = new Resource<AudioClip>(_bgm);
            playerPrefsKey = stageName + delimiter + _bgm + "_" + suffix;

            if (Prefs.HasKey(playerPrefsKey))
                _weight = Prefs.GetFloat(playerPrefsKey);
            else {
                Prefs.SetFloat(playerPrefsKey, _baseWeight);
                _weight = _baseWeight;
            }
        }

        public override string ToString() {
            return _bgm + " - (" + _baseWeight + ")";
        }

    }

}