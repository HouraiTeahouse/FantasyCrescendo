using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Hourai {

    public class LevelLoader : MonoBehaviour, ISubmitHandler {

        private enum Type {
            Awake,
            Start,
            Collision,
            UIEvent
        }

        [SerializeField]
        private LoadSceneMode _mode = LoadSceneMode.Single;

        [SerializeField]
        private Type _trigger;

        [SerializeField, Scene]
        private string[] _scenes;

        [SerializeField, Tooltip("Ignore if scenes are already loaded?")]
        private bool _ignoreLoadedScenes;

        public string[] Scenes {
            get { return _scenes; }
            set { _scenes = value; }
        }

        void Awake() {
            if (_trigger == Type.Awake)
                Load();
        }

        void Start() {
            if (_trigger == Type.Start)
                Load();
        }

        void OnTriggerEnter(Collider other) {
            if (_trigger == Type.Collision)
                Load();
        }

        public void OnSubmit(BaseEventData eventData) {
            if(_trigger == Type.UIEvent)
                Load();
        }

        public void Load() {
#if UNITY_EDITOR
            var scenes = new HashSet<string>(EditorSceneManager.GetSceneManagerSetup().Select(scene => scene.path));
#endif
            foreach (string scenePath in _scenes) {
#if UNITY_EDITOR
                if (scenes.Contains(scenePath) && !_ignoreLoadedScenes)
                    continue;
#endif
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.isLoaded && !_ignoreLoadedScenes)
                    continue;
                SceneManager.LoadScene(scenePath, _mode);
            }
        }

    }

}

