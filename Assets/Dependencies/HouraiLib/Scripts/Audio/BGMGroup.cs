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
        const string delimiter = "/";
        const string suffix = "weight";

        [SerializeField]
        [Tooltip("The artist who created this piece of music")]
        string _artist;

        [SerializeField]
        [Range(0f, 1f)]
        float _baseWeight = 1f;

        [SerializeField]
        [Resource(typeof(AudioClip))]
        string _bgm;

        Resource<AudioClip> _bgmResource;

        [SerializeField]
        [Tooltip("The sample number of the end point the loop.")]
        int _loopEnd;

        [SerializeField]
        [Tooltip("The sample number of the start point the loop.")]
        int _loopStart;

        [SerializeField]
        [Tooltip("The name of the BGM.")]
        string _name;

        PrefFloat _weight;

        public BGMData(string path, float weight) {
            _bgm = path;
            _baseWeight = weight;
        }

        public Resource<AudioClip> BGM {
            get { return _bgmResource; }
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
            _weight =
                new PrefFloat(string.Format("{0}{1}{2}_{3}",
                    stageName,
                    delimiter,
                    _bgm,
                    suffix));
        }

        public override string ToString() {
            return string.Format("{0} - ({1})", _bgm, _baseWeight);
        }
    }
}
