using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
            foreach (string scenePath in _scenes) {
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.isLoaded)
                    continue;
                SceneManager.LoadScene(scenePath, _mode);
            }
        }

    }

}

