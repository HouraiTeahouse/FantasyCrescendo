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

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;

#endif

namespace HouraiTeahouse {
    [CreateAssetMenu(fileName = "New BGM Group",
        menuName = "Hourai Teahouse/BGM Group")]
    public class BGMGroup : ScriptableObject {
        [SerializeField]
        [Tooltip("The name of the BGM group")]
        string _name;

        WeightedRNG<BGMData> _selection;

        [SerializeField]
        BGMData[] backgroundMusicData;

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

        protected virtual void OnDisable() {
            foreach (BGMData bgmData in backgroundMusicData) {
                bgmData.Finish(Name);
            }
        }

        public BGMData GetRandom() { return _selection.Select(); }

#if UNITY_EDITOR

        public void SetBGMClips(IEnumerable<string> resourcePaths) {
            backgroundMusicData =
                resourcePaths.Select(path => new BGMData(path, 1f)).ToArray();
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

        public Resource<AudioClip> BGM { get; private set; }
        public float Weight { get; private set; }

        public int LoopStart {
            get { return _loopStart; }
        }

        public int LoopEnd {
            get { return _loopEnd; }
        }

        string GetKey(string stageName) {
            return string.Format("{0}{1}{2}_{3}",  
                  stageName,
                  Delimiter,
                  _bgm,
                  Suffix);
        }

        public void Initialize(string stageName) {
            BGM = new Resource<AudioClip>(_bgm);
            var key = GetKey(stageName);
            if (Prefs.Exists(key))
                Weight = Prefs.GetFloat(key);
            else {
                Weight = _baseWeight;
                Prefs.SetFloat(key, Weight);
            }
        }

        public void Finish(string stageName) {
            Prefs.SetFloat(GetKey(stageName), Weight);
        }

        public override string ToString() {
            return string.Format("{0} - ({1})", _bgm, _baseWeight);
        }
    }
}
