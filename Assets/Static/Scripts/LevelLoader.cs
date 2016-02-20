using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse {

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
            HashSet<string> paths = new HashSet<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
                paths.Add(SceneManager.GetSceneAt(i).path);
            foreach (string scenePath in _scenes) {
                if (!_ignoreLoadedScenes && paths.Contains(string.Format("Assets/{0}.unity", scenePath)))
                    continue;
                SceneManager.LoadScene(scenePath, _mode);
            }
        }

    }

}

