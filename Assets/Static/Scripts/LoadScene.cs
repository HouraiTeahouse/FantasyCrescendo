using UnityEngine;

namespace Hourai {

    public class LoadScene : MonoBehaviour {

        [SerializeField]
        private string scene;
        
        void Start() {
            Application.LoadLevel(scene);
        }

    }

}

