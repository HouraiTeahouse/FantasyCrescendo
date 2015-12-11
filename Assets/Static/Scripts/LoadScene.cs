using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hourai {

    public class LoadScene : MonoBehaviour {

        [SerializeField]
        private string _scene;

        [SerializeField]
        private LoadSceneMode _mode = LoadSceneMode.Single;
        
        void Start() {
            SceneManager.LoadScene(_scene, _mode);
        }

    }

}

