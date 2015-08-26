using UnityEngine;

namespace Hourai.SmashBrew {

    /// <summary>
    /// A simple script that automatically starts the Match created automatically upon instantiation.
    /// Subsequently destroys itself as it has served its purpose.
    /// </summary>
    public class StartMatch : MonoBehaviour {

        /// <summary>
        /// Called on script instantiation.
        /// </summary>
        private void Start() {
            Match.Begin();
            Destroy(this);
        }

    }

}