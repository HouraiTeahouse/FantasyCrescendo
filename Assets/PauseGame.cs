using UnityEngine;
using Hourai.Events;

namespace Hourai.SmashBrew {

    public class PauseEvent : IEvent {



    }

    public class PauseGame : MonoBehaviour {

        [SerializeField]
        private KeyCode _key = KeyCode.Escape;

        void Update() {
            if (Input.GetKeyDown(_key))
                Game.Paused = !Game.Paused;
        }

    }

}