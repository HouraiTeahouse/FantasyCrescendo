using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        protected override void Action() { Load(); }

        /// <summary> Loads the scenes </summary>
        public void Load() {
            var paths = new HashSet<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++) {
                var path = SceneManager.GetSceneAt(i).path;
                paths.Add(path);
                Log.Debug(path);
            }
            foreach (string scenePath in _scenes) {
                if (!_ignoreLoadedScenes && paths.Contains("Assets/{0}.unity".With(scenePath)))
                    continue;
                SceneLoader.LoadScene(scenePath);
            }
        }

    }

}
