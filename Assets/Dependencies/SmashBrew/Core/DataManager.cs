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
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> A manager of all of the game data loaded into the game. </summary>
    public sealed class DataManager : MonoBehaviour {
        ReadOnlyCollection<CharacterData> _characterCollection;

        [SerializeField]
        [Tooltip("The characters to display in the game")]
        List<CharacterData> _characters;

        [SerializeField]
        [Tooltip("Destroy this instance on scene loads?")]
        bool _dontDestroyOnLoad;

        ReadOnlyCollection<SceneData> _sceneCollection;

        [SerializeField]
        [Tooltip("The scenes to show in the game")]
        List<SceneData> _scenes;

        public Mediator Mediator { get; private set; }

        /// <summary> The Singleton instance of DataManager. </summary>
        public static DataManager Instance { get; private set; }

        /// <summary> All Characters that are included with the Game's build. The Data Manager will automatically load all
        /// CharacterData instances from Resources. </summary>
        public ReadOnlyCollection<CharacterData> Characters {
            get { return _characterCollection; }
        }

        /// <summary> All Scenes and their metadata included with the game's build. The DataManager will automatically load all
        /// SceneData instances from Resources. </summary>
        public ReadOnlyCollection<SceneData> Scenes {
            get { return _sceneCollection; }
        }

        /// <summary> Unity Callback. Called on object instantion. </summary>
        void Awake() {
            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);

            Mediator = GlobalMediator.Instance;

            _characterCollection =
                new ReadOnlyCollection<CharacterData>(_characters);
            _sceneCollection = new ReadOnlyCollection<SceneData>(_scenes);

            SceneManager.sceneLoaded += SceneLoad;
        }

        void SceneLoad(Scene newScene, LoadSceneMode mode) {
            Log.Info("Unloading managed data assets");
            foreach (SceneData scene in _scenes)
                scene.Unload();
            foreach (CharacterData character in _characters) {
                Log.Debug(character);
                if(character != null)
                    character.Unload();
            }
        }
    }
}
