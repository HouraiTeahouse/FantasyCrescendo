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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse {
    /// <summary> A SingleActionBehaviour that loads new Scenes </summary>
    public class LevelLoader : SingleActionBehaviour {
        [SerializeField]
        [Tooltip("Ignore if scenes are already loaded?")]
        bool _ignoreLoadedScenes;

        [SerializeField,]
        [Tooltip("The mode to load scenes in")]
        LoadSceneMode _mode = LoadSceneMode.Single;

        [SerializeField]
        [Scene]
        [Tooltip("The target scenes to load")]
        string[] _scenes;

        /// <summary> The paths of the scenes to load </summary>
        public string[] Scenes {
            get { return _scenes; }
            set { _scenes = value; }
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() {
            Load();
        }

        /// <summary> Loads the scenes </summary>
        public void Load() {
            var paths = new HashSet<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
                paths.Add(SceneManager.GetSceneAt(i).path);
            foreach (string scenePath in _scenes) {
                if (!_ignoreLoadedScenes
                    && paths.Contains("Assets/{0}.unity".With(scenePath)))
                    continue;
                SceneManager.LoadScene(scenePath, _mode);
            }
        }
    }
}
