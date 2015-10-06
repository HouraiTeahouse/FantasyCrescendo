using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

namespace Hourai {

    public class LoadScene : BaseBehaviour {

        [SerializeField, Show, SelectScene]
        private string scene;
        
        void Start() {
            Application.LoadLevel(scene);
        }

    }

}

